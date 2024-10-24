using BL.Services;
using DAL.Models;
using Opc.Ua.Client;
using OpcUaClient;

namespace BL.Controls
{
    public abstract class DeviceControlWithHandlers : DeviceControl
    {
        protected internal void UnknownHandler(Opc.Ua.Client.MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                LogOpcUaMonitoredItemChange("unknown", value);

                TagReadDone[SectionDirectionEnum.reqDirect] = (bool)value.Value;
            }
        }

        protected internal void TagReadDoneFromDirectHandler(Opc.Ua.Client.MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                LogOpcUaMonitoredItemChange("tagReadDoneFromDirect", value);

                TagReadDone[SectionDirectionEnum.reqDirect] = (bool)value.Value;
            }
        }

        protected internal void TagIdFromRequestHandler(Opc.Ua.Client.MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                LogOpcUaMonitoredItemChange("tagIdFromRequest", value);

                short lastTagId = (short)value.Value;
                if (lastTagId != 0)
                {
                    TagId[SectionDirectionEnum.request] = lastTagId;
                }
            }
        }

        protected internal void TagIdFromSideHandler(Opc.Ua.Client.MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                LogOpcUaMonitoredItemChange("tagIdFromSide", value);

                short lastTagId = (short)value.Value;
                if (lastTagId != 0)
                {
                    TagId[SectionDirectionEnum.reqSide] = lastTagId;
                }
            }
        }

        protected internal void TagIdFromRightHandler(Opc.Ua.Client.MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                LogOpcUaMonitoredItemChange("tagIdFromRight", value);

                short lastTagId = (short)value.Value;
                if (lastTagId != 0)
                {
                    TagId[SectionDirectionEnum.reqRight] = lastTagId;
                }
            }
        }

        protected internal void TagIdFromDirectHandler(Opc.Ua.Client.MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                LogOpcUaMonitoredItemChange("tagIdFromDirect", value);

                short lastTagId = (short)value.Value;
                if (lastTagId != 0)
                {
                    TagId[SectionDirectionEnum.reqDirect] = lastTagId;
                }
            }
        }

        protected internal void TagIdFromLeftHandler(Opc.Ua.Client.MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                LogOpcUaMonitoredItemChange("tagIdFromLeft", value);

                short lastTagId = (short)value.Value;
                if (lastTagId != 0)
                {
                    TagId[SectionDirectionEnum.reqLeft] = lastTagId;
                }
            }
        }

        protected internal void EnabledHandler(Opc.Ua.Client.MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            DeviceServices deviceServices = new DeviceServices();
            foreach (var value in item.DequeueValues())
            {
                LogOpcUaMonitoredItemChange("enabled", value);

                deviceServices.UpdateEnabled(Id, (bool)value.Value);
            }
        }

        protected internal void HangerInHandler(Opc.Ua.Client.MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                LogOpcUaMonitoredItemChange("hangerIn", value);

                bool hangerIn = (bool)value.Value;
                switch (item.DisplayName.Split('.').Last())
                {
                    // PRIVATE - NOT PUBLISHED
                }
            }
        }

        protected internal async void AlarmGeneralHandler(Opc.Ua.Client.MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            DeviceServices deviceServices = new DeviceServices();
            AlarmServices alarmServices = new AlarmServices();

            foreach (var value in item.DequeueValues())
            {
                // PRIVATE - NOT PUBLISHED
            }
        }

        protected internal async void TransferedHandler(Opc.Ua.Client.MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                // PRIVATE - NOT PUBLISHED
            }
        }

        private Section GetTransferedPreviousSection()
        {
            SectionServices sectionServices = new SectionServices();
            return sectionServices.GetPreviousSectionOfDeviceFromDirect(Id, LastRequestFromDirect);
        }

        private Section GetTransferedNextSection()
        {
            SectionServices sectionServices = new SectionServices();
            return sectionServices.GetNextSectionOfDeviceToDirect(Id, LastRequestToDirect);
        }

        protected virtual void TransferHanger(OrderHangerSection? firstHangerInPreviousSection, Section nextSection, DateTime movedTimestamp)
        {
            OrderHangerSectionServices orderHangerSectionServices = new OrderHangerSectionServices();

            if (firstHangerInPreviousSection != null)
            {
                orderHangerSectionServices.AddOrUpdateOrderHangerSection(firstHangerInPreviousSection.OrderHangerId, nextSection, movedTimestamp);
                orderHangerSectionServices.LeftSection(firstHangerInPreviousSection, movedTimestamp);
            }
        }

        protected internal virtual async Task DiscardRequestAsync(int delayAfterDiscartingRequest = 2000)
        {
            LogServices logServices = new LogServices();
            logServices.AddLog("request", "request", NodeId, "false");

            await OpcUaClientManager.Instance.WriteValueAsync(NodeId + @".""i"".""request""", false);
            Requested = false;
            await Task.Delay(delayAfterDiscartingRequest);
        }

        private void LogOpcUaMonitoredItemChange(string action, Opc.Ua.DataValue? value)
        {
            LogServices logServices = new LogServices();
            string? valueString = value?.Value?.ToString();
            if (valueString != null)
            {
                logServices.AddLog(action, Name, valueString);
                //Console.WriteLine(@"{0}: {1}.""o"".""tagID"": {2}, {3}, {4}", Name, NodeId, value.Value, value.SourceTimestamp, value.StatusCode);
            }
            else {
                logServices.AddLog(action, Name, "null");
            }
        }
    }
}
