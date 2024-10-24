using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using OpcUaClient.Models;

namespace OpcUaClient
{
    public sealed class OpcUaClientManager
    {
        private const string OpcUaServerUrl = ""; // PRIVATE - NOT PUBLISHED

        private static readonly object createLock = new object();
        private static OpcUaClientManager? instance = null;
        public static OpcUaClientManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (createLock)
                    {
                        if (instance == null)
                        {
                            instance = new OpcUaClientManager();
                        }
                    }
                }
                return instance;
            }
        }

        private OpcUaClientManager()
        {
            CreateConfiguration();
            CreateSession();
        }

        private ApplicationConfiguration Config { get; set; } = new ApplicationConfiguration()
        {
            ApplicationName = "OpcUaClient",
            ApplicationUri = Utils.Format(@"urn:{0}:OpcUaClient", System.Net.Dns.GetHostName()),
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = "OPC_UA_Client" },
                TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
                AutoAcceptUntrustedCertificates = true
            },
            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
            TraceConfiguration = new TraceConfiguration()
        };


        private static readonly object createSessionLock = new object();

        private Session? Session { get; set; }

        private SessionReconnectHandler? SessionReconnectHandler { get; set; } = new SessionReconnectHandler(true, 10*1000);

        private void CreateConfiguration()
        {
            Config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
            if (Config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                Config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted); };
            }

            ApplicationInstance application = new ApplicationInstance
            {
                ApplicationName = "OPC_UA_Client",
                ApplicationType = ApplicationType.Client,
                ApplicationConfiguration = Config
            };

            application.CheckApplicationInstanceCertificate(false, 2048).GetAwaiter().GetResult();
        }

        private void CreateSession()
        {
            if (Session == null)
            {
                lock (createSessionLock)
                {
                    if (Session == null)
                    {
                        while (true)
                        {
                            try
                            {
                                EndpointDescription selectedEndpoint = CoreClientUtils.SelectEndpoint(OpcUaServerUrl, useSecurity: false, discoverTimeout: 5000);
                                ConfiguredEndpoint configuredEndpoint = new ConfiguredEndpoint(null, selectedEndpoint, EndpointConfiguration.Create(Config));
                                Session = Session.Create(Config, configuredEndpoint, false, "OpcUaClientAppSession", sessionTimeout: 60000, null, null).GetAwaiter().GetResult();
                                Session.KeepAlive += KeepAliveSession;
                                if (Session != null)
                                {
                                    break; // Exit - the session successfully created.
                                }
                            }
                            catch
                            {
                                Console.WriteLine("OPC UA Client: Failed to create session with PLC (OPC UA server).");
                            }
                        }
                    }
                }
            }
        }

        private void KeepAliveSession(ISession session, KeepAliveEventArgs e)
        {
            if (ServiceResult.IsBad(e.Status))
            {
                Console.WriteLine("OPC UA Client: Session keep alive error. Trying to reconnect.");
                SessionReconnectHandler?.BeginReconnect(session, 1000, ReconnectComplete);
            }
        }

        private void ReconnectComplete(object? sender, EventArgs e)
        {
            if (SessionReconnectHandler?.Session != null)
            {
                if(!ReferenceEquals(Session, SessionReconnectHandler.Session))
                {
                    var session_tmp = Session;
                    session_tmp.KeepAlive -= KeepAliveSession;
                    Session = SessionReconnectHandler.Session as Session;
                    Session.KeepAlive += KeepAliveSession;
                    Utils.SilentDispose(session_tmp);
                }
            }
        }

        public void AddListeners(List<OpcUaSubscription> opcUaSubscriptions)
        {
            if (Session == null)
            {
                CreateSession();
                throw new Exception("OPC UA Client: Session is null.");
            }

            try
            {
                Subscription subscription = new Subscription(Session.DefaultSubscription)
                {
                    PublishingEnabled = true,
                    PublishingInterval = 500
                };
                List<MonitoredItem> list = new List<MonitoredItem>();

                foreach (OpcUaSubscription opcUaSubscription in opcUaSubscriptions)
                {
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = opcUaSubscription.DisplayName,
                        StartNodeId = opcUaSubscription.NodeId,
                        SamplingInterval = 500,
                        QueueSize = 1,
                        DiscardOldest = true
                    });
                    list.Last().Notification += opcUaSubscription.Handler;
                }

                subscription.AddItems(list);

                Session.AddSubscription(subscription);
                subscription.Create();
            }
            catch
            {
                throw;
            }
        }

        public async Task WriteValueAsync(string nodeId, object value)
        {
            try
            {
                await WriteValuesAsync(new List<string> { nodeId }, value);
            }
            catch
            {
                throw;
            }
        }

        // Attribute IDs: https://opclabs.doc-that.com/files/onlinedocs/QuickOpc/Latest/User%27s%20Guide%20and%20Reference-QuickOPC/OpcLabs.EasyOpcUA~OpcLabs.EasyOpc.UA.UAAttributeId.html
        public async Task WriteValuesAsync(List<string> nodeIds, object value)
        {
            WriteValueCollection valuesToWrite = new WriteValueCollection();

            foreach (var nodeId in nodeIds)
            {
                valuesToWrite.Add(new WriteValue()
                {
                    NodeId = nodeId,
                    AttributeId = 13,
                    Value = new()
                    {
                        Value = value
                    }

                });
            }

            StatusCodeCollection results = new StatusCodeCollection();
            DiagnosticInfoCollection diagnosticInfo = new DiagnosticInfoCollection();

            RequestHeader requestHeader = new RequestHeader();
            CancellationToken cancellationToken = new CancellationToken();

            WriteResponse writeResponse = new WriteResponse();

            if (Session == null)
            {
                CreateSession();
            }

            if (Session != null)
            {
                try
                {
                    writeResponse = await Session.WriteAsync(requestHeader, valuesToWrite, cancellationToken);
                }
                catch
                {
                    Console.WriteLine("Error while WriteValuesAsync in OpcUaClientManager:");
                    Console.WriteLine(string.Join(", ", nodeIds));
                    throw;
                }
            }
            else
            {
                throw new NullReferenceException("Session is null in WriteValueAsync.");
            }

            foreach (var result in writeResponse.Results)
            {
                if (StatusCode.IsBad(result))
                {
                    Console.WriteLine("Error while WriteValuesAsync in OpcUaClientManager:");
                    Console.WriteLine(string.Join(", ", nodeIds));
                    Console.WriteLine(result);
                }
            }
        }

        public async Task<List<T>> ReadValuesAsync<T>(List<string> nodeIds, T valueType)
        {
            try
            {
                (DataValueCollection, IList<ServiceResult>) readResult = await ReadValuesAsync(nodeIds);
                List<T> values = new List<T>();
                foreach (var value in readResult.Item1)
                {
                    values.Add(value.GetValue(valueType));
                }

                return values;
            }
            catch
            {
                throw;
            }
        }

        public async Task<T> ReadValueAsync<T>(string nodeId, T valueType)
        {
            try
            {
                (DataValueCollection, IList<ServiceResult>) readResult = await ReadValuesAsync(new List<string> { nodeId });
                return readResult.Item1[0].GetValue(valueType);
            }
            catch
            {
                throw;
            }
        }

        private async Task<(DataValueCollection, IList<ServiceResult>)> ReadValuesAsync(List<string> nodeIds)
        {
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

            foreach (var nodeId in nodeIds)
            {
                nodesToRead.Add(new ReadValueId()
                {
                    NodeId = nodeId,
                    AttributeId = 13,
                });
            }

            StatusCodeCollection results = new StatusCodeCollection();
            DiagnosticInfoCollection diagnosticInfo = new DiagnosticInfoCollection();

            RequestHeader requestHeader = new RequestHeader();
            TimestampsToReturn timestampsToReturn = new TimestampsToReturn();
            CancellationToken cancellationToken = new CancellationToken();

            if (Session == null)
            {
                CreateSession();
            }

            if (Session != null)
            {
                try
                {
                    var errors = new List<ServiceResult>(nodesToRead.Count);

                    ReadResponse readResponse = await Session.ReadAsync(requestHeader, 0, timestampsToReturn, nodesToRead, cancellationToken);

                    DataValueCollection values = readResponse.Results;
                    diagnosticInfo = readResponse.DiagnosticInfos;

                    ClientBase.ValidateResponse(values, nodesToRead);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfo, nodesToRead);

                    foreach (var value in values)
                    {
                        ServiceResult result = ServiceResult.Good;
                        if (StatusCode.IsBad(value.StatusCode))
                        {
                            result = ClientBase.GetResult(values[0].StatusCode, 0, diagnosticInfo, readResponse.ResponseHeader);
                            Console.WriteLine("Error while ReadValuesAsync in OpcUaClientManager:");
                            Console.WriteLine(string.Join(", ", nodeIds));
                            Console.WriteLine(result);
                        }
                        errors.Add(result);
                    }

                    return (values, errors);
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                throw new NullReferenceException("Session is null in ReadValueAsync.");
            }
        }


        public List<string> GetChildNodes(string nodeId)
        {
            // Create the necessary objects and variables
            BrowseDescriptionCollection nodesToBrowse = new BrowseDescriptionCollection();

            // Add the browse descriptions to the collection
            nodesToBrowse.Add(new BrowseDescription()
            {
                NodeId = nodeId, // Specify the NodeId of the node you want to browse
                BrowseDirection = BrowseDirection.Forward, // Specify the direction of the browse operation
                ReferenceTypeId = ReferenceTypeIds.Aggregates, // Specify the reference type to follow
                IncludeSubtypes = true, // Specify whether to include subtypes of the reference type
                NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable), // Specify the node class(es) to include in the results
                ResultMask = (uint)BrowseResultMask.All // Specify the information to include in the results
            });

            List<string> childNodes = new List<string>();

            if (Session == null)
            {
                CreateSession();
            }

            if (Session != null)
            {
                try
                {
                    ReferenceDescriptionCollection references = new ReferenceDescriptionCollection();
                    BrowseDescriptionCollection unprocessedOperations = new BrowseDescriptionCollection();

                    while (nodesToBrowse.Count > 0)
                    {
                        // start the browse operation.
                        BrowseResultCollection results = null;
                        DiagnosticInfoCollection diagnosticInfos = null;

                        Session.Browse(
                            null,
                            null,
                            0,
                            nodesToBrowse,
                            out results,
                            out diagnosticInfos);

                        ClientBase.ValidateResponse(results, nodesToBrowse);
                        ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToBrowse);

                        ByteStringCollection continuationPoints = new ByteStringCollection();

                        for (int ii = 0; ii < nodesToBrowse.Count; ii++)
                        {
                            // check for error.
                            if (StatusCode.IsBad(results[ii].StatusCode))
                            {
                                // this error indicates that the server does not have enough simultaneously active 
                                // continuation points. This request will need to be resent after the other operations
                                // have been completed and their continuation points released.
                                if (results[ii].StatusCode == StatusCodes.BadNoContinuationPoints)
                                {
                                    unprocessedOperations.Add(nodesToBrowse[ii]);
                                }

                                continue;
                            }

                            // check if all references have been fetched.
                            if (results[ii].References.Count == 0)
                            {
                                continue;
                            }

                            // save results.
                            references.AddRange(results[ii].References);

                            // check for continuation point.
                            if (results[ii].ContinuationPoint != null)
                            {
                                continuationPoints.Add(results[ii].ContinuationPoint);
                            }
                        }

                        // process continuation points.
                        ByteStringCollection revisedContinuationPoints = new ByteStringCollection();

                        while (continuationPoints.Count > 0)
                        {
                            // continue browse operation.
                            Session.BrowseNext(
                                null,
                                false,
                                continuationPoints,
                                out results,
                                out diagnosticInfos);

                            ClientBase.ValidateResponse(results, continuationPoints);
                            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, continuationPoints);

                            for (int ii = 0; ii < continuationPoints.Count; ii++)
                            {
                                // check for error.
                                if (StatusCode.IsBad(results[ii].StatusCode))
                                {
                                    continue;
                                }

                                // check if all references have been fetched.
                                if (results[ii].References.Count == 0)
                                {
                                    continue;
                                }

                                // save results.
                                references.AddRange(results[ii].References);

                                // check for continuation point.
                                if (results[ii].ContinuationPoint != null)
                                {
                                    revisedContinuationPoints.Add(results[ii].ContinuationPoint);
                                }
                            }

                            // check if browsing must continue;
                            continuationPoints = revisedContinuationPoints;
                        }

                        // check if unprocessed results exist.
                        nodesToBrowse = unprocessedOperations;
                    }




                    // process results.
                    for (int ii = 0; ii < references.Count; ii++)
                    {
                        ReferenceDescription target = references[ii];

                        // add node.
                        childNodes.Add(target.NodeId.ToString());
                    }
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                throw new NullReferenceException("Session is null in ReadValueAsync.");
            }
            return childNodes;
        }
    }
}
