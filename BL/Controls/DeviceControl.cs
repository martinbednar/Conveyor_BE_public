using BL.Models.DTOs;
using BL.Services;
using DAL.Models;
using Microsoft.IdentityModel.Tokens;
using OpcUaClient;

namespace BL.Controls
{
    public abstract class DeviceControl : DeviceDto
    {
        protected IDictionary<SectionDirectionEnum, short> TagId { get; set; } = new Dictionary<SectionDirectionEnum, short>()
        {
            { SectionDirectionEnum.request, 0 },
            { SectionDirectionEnum.reqDirect, 0 },
            { SectionDirectionEnum.reqSide, 0 },
            { SectionDirectionEnum.reqLeft, 0 },
            { SectionDirectionEnum.reqRight, 0 }
        };
        protected IDictionary<SectionDirectionEnum, short> LastTagId { get; set; } = new Dictionary<SectionDirectionEnum, short>()
        {
            { SectionDirectionEnum.request, -1 },
            { SectionDirectionEnum.reqDirect, -1 },
            { SectionDirectionEnum.reqSide, -1 },
            { SectionDirectionEnum.reqLeft, -1 },
            { SectionDirectionEnum.reqRight, -1 }
        };
        protected IDictionary<SectionDirectionEnum, bool> TagReadDone { get; set; } = new Dictionary<SectionDirectionEnum, bool>()
        {
            { SectionDirectionEnum.request, false },
            { SectionDirectionEnum.reqDirect, false },
            { SectionDirectionEnum.reqSide, false },
            { SectionDirectionEnum.reqLeft, false },
            { SectionDirectionEnum.reqRight, false }
        };
        protected virtual List<SectionDirectionEnum> DirectsOfPreviousSectionsWithRfid { get; set; } = new List<SectionDirectionEnum>();
        protected List<Section> ForbiddenPreviousSections { get; set; } = new List<Section>();
        protected IDictionary<SectionDirectionEnum, bool> HangerIn { get; set; } = new Dictionary<SectionDirectionEnum, bool>()
        {
            { SectionDirectionEnum.request, false },
            { SectionDirectionEnum.reqDirect, false },
            { SectionDirectionEnum.reqSide, false },
            { SectionDirectionEnum.reqLeft, false },
            { SectionDirectionEnum.reqRight, false }
        };
        protected bool TransferedOld { get; set; } = true;
        protected virtual bool Requested { get; set; } = false;
        protected Section? PreviousSection { get; set; }
        protected virtual SectionDirectionEnum LastRequestFromDirect { get; set; } = SectionDirectionEnum.request;
        protected virtual SectionDirectionEnum LastRequestToDirect { get; set; } = SectionDirectionEnum.request;


        protected internal async void TryToMoveHangerAsync()
        {
            // PRIVATE - NOT PUBLISHED
        }

        private void CorrectByRfidHeaders()
        {
            // PRIVATE - NOT PUBLISHED
        }

        protected virtual void SelectPreviousSectionForTransfer()
        {
            PreviousSection = null;
            SectionServices sectionServices = new SectionServices();
            Section previousSection = sectionServices.GetFirstPreviousSectionOfDevice(Id);
            if (!ForbiddenPreviousSections.Contains(previousSection, new SectionComparer()))
            {
                PreviousSection = previousSection;
            }
        }

        protected virtual void InsertOrderHangerFuturePath(int orderHangerId)
        {
            OrderHangerSectionServices orderHangerSectionServices = new OrderHangerSectionServices();
            SectionServices sectionServices = new SectionServices();

            List<Section> futurePath = new List<Section>()
            {
                sectionServices.GetFirstNextSectionOfDevice(Id)
            };
            orderHangerSectionServices.InsertFuturePath(orderHangerId, futurePath);
        }

        protected void CheckOrderHangerFuturePath(int orderHangerId)
        {
            OrderHangerSectionServices orderHangerSectionServices = new OrderHangerSectionServices();

            if (orderHangerSectionServices.GetFuturePathOfHanger(orderHangerId).IsNullOrEmpty())
            {
                InsertOrderHangerFuturePath(orderHangerId);
            }
        }

        protected virtual bool GetPermit(Section nextSection)
        {
            SectionServices sectionServices = new SectionServices();
            return sectionServices.SectionIsNotFull(nextSection);
        }

        protected Section? GetNextSection(int orderHangerId)
        {
            OrderHangerSectionServices orderHangersectionServices = new OrderHangerSectionServices();
            List<OrderHangerSection> futurePath = orderHangersectionServices.GetFuturePathOfHanger(orderHangerId);
            if (futurePath.Count > 0)
            {
                return futurePath.First().Section;
            }
            else
            {
                return null;
            }
        }

        protected virtual Section GetFirstNextSection()
        {
            SectionServices sectionServices = new SectionServices();
            return sectionServices.GetFirstNextSectionOfDevice(Id);
        }

        private async Task RequestAsync(Section previousSection, Section nextSection)
        {
            SectionDirectionEnum previousSectionDirect = (SectionDirectionEnum)previousSection.SectionDirectionToOutputDeviceId;
            SectionDirectionEnum nextSectionDirect = (SectionDirectionEnum)nextSection.SectionDirectionFromInputDeviceId;

            try
            {
                await SendRequestAsync(previousSectionDirect, nextSectionDirect);
            }
            catch
            {
                throw;
            }

            LastRequestFromDirect = previousSectionDirect;
            LastRequestToDirect = nextSectionDirect;
        }

        protected virtual async Task SendRequestAsync(SectionDirectionEnum previousSectionDirect, SectionDirectionEnum nextSectionDirect)
        {
            try
            {
                LogServices logServices = new LogServices();
                logServices.AddLog("request", nextSectionDirect.ToString(), NodeId, "true");

                await OpcUaClientManager.Instance.WriteValueAsync(NodeId + @".""i"".""" + nextSectionDirect.ToString() + @"""", true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected virtual bool IsHangerWaiting()
        {
            return HangerIn[SectionDirectionEnum.request];
        }
    }
}
