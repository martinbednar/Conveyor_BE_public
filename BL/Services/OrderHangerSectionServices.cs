using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BL.Services
{
    public class OrderHangerSectionServices
    {
        MyDbContext dbContext = new();

        internal OrderHangerSection? GetOrderHangerSection(int orderHangerId, int sectionId)
        {
            return dbContext.OrderHangerSections.Where(ohs => ohs.OrderHangerId == orderHangerId).Where(ohs => ohs.SectionId == sectionId).OrderByDescending(ohs => ohs.Id).FirstOrDefault();
        }

        internal OrderHangerSection? GetCurrectSectionOfOrderHanger(int orderHangerId)
        {
            return dbContext.OrderHangerSections.Where(ohs => ohs.OrderHangerId == orderHangerId).Where(ohs => ohs.Entered != null).Where(ohs => ohs.Left == null).OrderByDescending(ohs => ohs.Id).FirstOrDefault();
        }

        public void AddOrUpdateOrderHangerSection(int orderHangerId, Section section, DateTime entered)
        {
            OrderHangerSection? orderHangerSectionAlreadyInDb = GetOrderHangerSection(orderHangerId, section.Id);
            if (orderHangerSectionAlreadyInDb != null)
            {
                orderHangerSectionAlreadyInDb.Entered = entered;
                orderHangerSectionAlreadyInDb.Left = null;
                dbContext.SaveChanges();
                return;
            }

            dbContext.OrderHangerSections.Add(new OrderHangerSection()
            {
                OrderHangerId = orderHangerId,
                SectionId = section.Id,
                Entered = entered,
                Left = null
            });
            dbContext.SaveChanges();
        }

        public void LeftSection(OrderHangerSection orderHangerSection, DateTime left)
        {
            orderHangerSection.Left = left;

            dbContext.OrderHangerSections.Update(orderHangerSection);

            dbContext.SaveChanges();
        }

        public List<OrderHangerSection> GetHangersInSection(Section section)
        {
            return dbContext.OrderHangerSections.Include(ohs => ohs.OrderHanger).Where(ohs => ohs.Section == section).Where(ohs => ohs.Entered != null).Where(ohs => ohs.Left == null).OrderBy(ohs => ohs.Entered).ToList();
        }

        public List<OrderHangerSection> GetHangersGoingToSection(Section section)
        {
            return dbContext.OrderHangerSections.Include(ohs => ohs.OrderHanger).Where(ohs => ohs.Section == section).Where(ohs => ohs.Left == null).ToList();
        }

        public OrderHangerSection? GetFirstHangerInSection(Section section)
        {
            return dbContext.OrderHangerSections.Include(ohs => ohs.OrderHanger).Where(ohs => ohs.Section == section).Where(ohs => ohs.Entered != null).Where(ohs => ohs.Left == null).OrderBy(ohs => ohs.Entered).FirstOrDefault();
        }

        internal List<OrderHangerSection> GetFuturePathOfHanger(int orderHangerId)
        {
            int currentSectionOfOrderHangerId = 0;
            OrderHangerSection? currentSectionOfOrderHanger = GetCurrectSectionOfOrderHanger(orderHangerId);
            if (currentSectionOfOrderHanger != null)
            {
                currentSectionOfOrderHangerId = currentSectionOfOrderHanger.Id;
            }
            return dbContext.OrderHangerSections.Include(ohs => ohs.Section).Where(ohs => ohs.OrderHangerId == orderHangerId).Where(ohs => ohs.Entered == null).Where(ohs => ohs.Id > currentSectionOfOrderHangerId).OrderBy(ohs => ohs.Index).ToList();
        }

        internal void InsertFuturePath(int orderHangerId, List<Section> futurePath)
        {
            int index = 0;
            foreach (var section in futurePath)
            {
                OrderHangerSection futureSection = new OrderHangerSection()
                {
                    OrderHangerId = orderHangerId,
                    SectionId = section.Id,
                    Entered = null,
                    Index = index
                };

                // Do not insert duplicit section to future path.
                // Delete the same future section if already exists.
                dbContext.OrderHangerSections.Where(ohs =>
                    (ohs.OrderHangerId == futureSection.OrderHangerId) &&
                    (ohs.SectionId == futureSection.SectionId) &&
                    (ohs.Entered == null)
                ).ExecuteDelete();

                dbContext.OrderHangerSections.Add(futureSection);
                index++;
            }
            dbContext.SaveChanges();
        }

        public void CancelAllNotLeftSectionsForOrderHanger(int orderHangerId)
        {
            List<OrderHangerSection> notLeftSections = dbContext.OrderHangerSections.Where(ohs => (ohs.OrderHangerId == orderHangerId) && (ohs.Left == null)).ToList();

            foreach (var notLeftSection in notLeftSections)
            {
                if (notLeftSection.Entered == null)
                {
                    notLeftSection.Entered = DateTime.MinValue;
                }
                notLeftSection.Left = DateTime.MinValue;
            }
            dbContext.SaveChanges();
        }

        public void CancelAllNotEnteredSectionsForOrderHanger(int orderHangerId)
        {
            List<OrderHangerSection> notEnteredSections = dbContext.OrderHangerSections.Where(ohs => (ohs.OrderHangerId == orderHangerId) && (ohs.Entered == null)).ToList();

            foreach (var notEnteredSection in notEnteredSections)
            {
                notEnteredSection.Entered = DateTime.MinValue;
                notEnteredSection.Left = DateTime.MinValue;
            }
            dbContext.SaveChanges();
        }

        internal void MoveHangerOnFirstPositionToSection(short hangerId, Section section)
        {
            OrderHangerServices orderHangerServices = new OrderHangerServices();
            OrderHangerSectionServices orderHangerSectionServices = new OrderHangerSectionServices();
            SectionServices sectionServices = new SectionServices();
            PathServices pathServices = new PathServices();

            OrderHangerSection? firstHangerInPreviousSection = orderHangerSectionServices.GetFirstHangerInSection(section);
            if (firstHangerInPreviousSection != null)
            {
                orderHangerSectionServices.CancelAllNotLeftSectionsForOrderHanger(firstHangerInPreviousSection.OrderHangerId);
                Section depotSection = sectionServices.GetSectionById((int)StationEnum.Depot);
                AddOrUpdateOrderHangerSection(firstHangerInPreviousSection.OrderHangerId, depotSection, DateTime.Now);
            }

            OrderHanger? orderHanger = orderHangerServices.GetActiveOrderHangerByHangerId(hangerId);
            if (orderHanger != null)
            {
                OrderHangerSection? orderHangerGoal = GetFuturePathOfHanger(orderHanger.Id).LastOrDefault();

                CancelAllNotLeftSectionsForOrderHanger(orderHanger.Id);

                OrderHangerSection? firstHangerInSection = GetFirstHangerInSection(section);
                if (firstHangerInSection != null)
                {
                    DateTime? firstHangerInSectionEntered = firstHangerInSection.Entered;
                    if (firstHangerInSectionEntered == null)
                    {
                        AddOrUpdateOrderHangerSection(orderHanger.Id, section, DateTime.MinValue);
                    }
                    else
                    {
                        AddOrUpdateOrderHangerSection(orderHanger.Id, section, ((DateTime)firstHangerInSectionEntered).AddSeconds(-1));
                    }
                }
                else
                {
                    AddOrUpdateOrderHangerSection(orderHanger.Id, section, DateTime.Now);
                }

                if (orderHangerGoal != null)
                {
                    List<Section> newFuturePath = pathServices.GetPath(section, orderHangerGoal.Section);
                    if (newFuturePath != null)
                    {
                        InsertFuturePath(orderHanger.Id, newFuturePath);
                    }
                    else
                    {
                        PlanPathToNearestStationOrOut(orderHanger, section);
                    }
                }
                else
                {
                    PlanPathToNearestStationOrOut(orderHanger, section);
                }
            }
            else
            {
                PlanPathToGoOut(hangerId, section);
            }
        }

        private void PlanPathToGoOut(short hangerId, Section section)
        {
            OrderHangerSectionServices orderHangerSectionServices = new OrderHangerSectionServices();
            SectionServices sectionServices = new SectionServices();
            OrderHangerServices orderHangerServices = new OrderHangerServices();
            PathServices pathServices = new PathServices();

            DateTime movedTimestamp = DateTime.Now;
            OrderHangerSection? firstHangerInSection = GetFirstHangerInSection(section);
            if (firstHangerInSection != null)
            {
                DateTime? firstHangerInSectionEntered = firstHangerInSection.Entered;
                if (firstHangerInSectionEntered == null)
                {
                    movedTimestamp = DateTime.MinValue;
                }
                else
                {
                    movedTimestamp = ((DateTime)firstHangerInSectionEntered).AddSeconds(-1);
                }
            }

            OrderHanger newOrderHanger = orderHangerServices.AddOrderHanger(1, hangerId, movedTimestamp, OrderHangerTypeEnum.Production);
            orderHangerSectionServices.AddOrUpdateOrderHangerSection(newOrderHanger.Id, section, movedTimestamp);

            List<Section> newFuturePath = pathServices.GetPath(section, sectionServices.GetSectionById(421431));
            if (newFuturePath == null)
            {
                newFuturePath = pathServices.GetPath(section, sectionServices.GetSectionById(411431));
            }
            if (newFuturePath != null)
            {
                InsertFuturePath(newOrderHanger.Id, newFuturePath);
            }
        }

        private void PlanPathToNearestStationOrOut(OrderHanger orderHanger, Section section)
        {
            PathServices pathServices = new PathServices();
            SectionServices sectionServices = new SectionServices();
            OrderServices orderServices = new OrderServices();

            List<Section> orderStations = orderServices.GetOrderStations(orderHanger.OrderId);
            orderStations.RemoveAll(s => (s.Id == (int)StationEnum.Tubing1) || (s.Id == (int)StationEnum.Tubing2) || (s.Id == (int)StationEnum.Tubing3) || (s.Id == (int)StationEnum.Tubing4));

            // PRIVATE - NOT PUBLISHED

            List<SectionWrapper> orderStationsWithNumberOfHangers = new List<SectionWrapper>();

            foreach (Section orderStation in orderStations)
            {
                orderStationsWithNumberOfHangers.Add(new SectionWrapper()
                {
                    Section = orderStation,
                    NumberOfHangersGoingToSection = GetHangersGoingToSection(orderStation).Count()
                });
            }

            orderStationsWithNumberOfHangers = orderStationsWithNumberOfHangers.OrderBy(s => s.Section.StationTypeId).ThenBy(s => s.NumberOfHangersGoingToSection).ThenByDescending(s => s.Section.Id).ToList();

            List<Section>? newFuturePath = null;

            foreach (SectionWrapper orderStation in orderStationsWithNumberOfHangers)
            {
                if (newFuturePath == null)
                {
                    newFuturePath = pathServices.GetPath(section, sectionServices.GetSectionById(orderStation.Section.Id));
                }
            }

            if (newFuturePath == null)
            {
                newFuturePath = pathServices.GetPath(section, sectionServices.GetSectionById(421431));
            }

            if (newFuturePath == null)
            {
                newFuturePath = pathServices.GetPath(section, sectionServices.GetSectionById(411431));
            }

            if (newFuturePath != null)
            {
                InsertFuturePath(orderHanger.Id, newFuturePath);
            }
        }

        internal void LeftCurrentSection(int orderHangerId, DateTime timestamp)
        {
            OrderHangerSection? orderHangerCurrentSection = dbContext.OrderHangerSections.Where(ohs => (ohs.OrderHangerId == orderHangerId) && (ohs.Entered != null) && (ohs.Left == null)).FirstOrDefault();
            if (orderHangerCurrentSection != null)
            {
                orderHangerCurrentSection.Left = timestamp;
                dbContext.SaveChanges();
            }
        }

        public void DeleteAllNonPackedOrderHangersFromOrder(int orderId)
        {
            OrderHangerServices orderHangerServices = new OrderHangerServices();
            List<OrderHanger> nonpackedOrderHangers = orderHangerServices.GetNonPackedOrderHangersForOrder(orderId);

            foreach (OrderHanger nonpackedOrderHanger in nonpackedOrderHangers)
            {
                CancelAllNotLeftSectionsForOrderHanger(nonpackedOrderHanger.Id);
                orderHangerServices.DeleteOrderHanger(nonpackedOrderHanger.Id);
            }
        }
    }
}
