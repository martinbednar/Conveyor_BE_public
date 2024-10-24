using DAL.Models;

namespace BL.Services
{
    public class PathServices
    {
        public List<Section> GetPath(Section from, Section to)
        {
            SectionServices sectionServices = new SectionServices();
            List<Section> sections = sectionServices.GetAllSections();

            // Make graph acyclic
            sections.RemoveAll(s => (s.Id == 111112) || (s.Id == 121122) || (s.Id == 131132) || (s.Id == 141142));

            // Initialize all the distances to max, and the "previous" city to null
            // [sectionId] = (previousSection, distance)
            Dictionary<int, (Section? previousSection, int distance)> distances = new Dictionary<int, (Section?, int)>();
            foreach (var section in sections)
            {
                distances[section.Id] = (null, int.MaxValue);
            }

            // priority queue for tracking shortest distance from the start node to each other node
            List<Section> queue = new List<Section>();

            // initialize the start node at a distance of 0
            distances[from.Id] = (null, 0);

            // add the start node to the queue for processing
            queue.Add(from);

            // as long as we have a node to process, keep looping
            while (queue.Count > 0)
            {
                // remove the node with the current smallest distance from the start node
                Section current = queue.First();
                queue.Remove(current);

                // if this is the node we want, then we're finished
                // as we must already have the shortest route!
                if (current.Id == to.Id)
                {
                    // build the route by tracking back through previous
                    return BuildRoute(distances, to);
                }

                foreach (var section in sections.Where(s => s.InputDeviceId == current.OutputDeviceId))
                {
                    // get the current shortest distance to the connected node
                    int distance = distances[section.Id].distance;
                    // calculate the new cumulative distance to the edge
                    int newDistance = distances[current.Id].distance + 1;

                    // if the new distance is shorter, then it represents a new 
                    // shortest-path to the connected edge
                    if (newDistance < distance)
                    {
                        // update the shortest distance to the connection
                        // and record the "current" node as the shortest
                        // route to get there 
                        distances[section.Id] = (current, newDistance);

                        // if the node is already in the queue, first remove it
                        queue.Remove(section);
                        // now add the node with the new distance
                        queue.Add(section);
                    }
                }
            }

            // if we don't have anything left, then we've processed everything,
            // but didn't find the node we want
            return null;
        }

        private List<Section> BuildRoute(Dictionary<int, (Section? previousSection, int distance)> distances, Section endSection)
        {
            var route = new List<Section>();
            Section? prev = endSection;

            // Keep examining the previous version until we
            // get back to the start node
            while (prev is not null)
            {
                var current = prev;
                prev = distances[current.Id].previousSection;
                route.Add(current);
            }

            route.RemoveAt(route.Count - 1);
            // reverse the route
            route.Reverse();
            return route;
        }

        public void InsertOrderHangerFuturePath(int orderHangerId, Section from, List<StationTypeEnum> toStationTypes)
        {
            OrderHangerServices orderHangerServices = new OrderHangerServices();
            OrderHangerSectionServices orderHangerSectionServices = new OrderHangerSectionServices();

            OrderHanger orderHanger = orderHangerServices.GetOrderHanger(orderHangerId);

            foreach (StationTypeEnum toStationType in toStationTypes)
            {
                InsertOrderHangerFuturePath(orderHangerId, from, toStationType);
                if (orderHangerSectionServices.GetFuturePathOfHanger(orderHangerId).Count() > 0)
                {
                    break;
                }
            }
        }

        internal void InsertOrderHangerFuturePath(int orderHangerId, Section from, StationTypeEnum toStationType)
        {
            OrderHangerSectionServices orderHangerSectionServices = new OrderHangerSectionServices();
            OrderHangerServices orderHangerServices = new OrderHangerServices();
            PathServices pathServices = new PathServices();
            OrderStationsServices orderStationsServices = new OrderStationsServices();
            SectionServices sectionServices = new SectionServices();

            OrderHanger orderHanger = orderHangerServices.GetOrderHanger(orderHangerId);
            List<Section> toStations = orderStationsServices.GetActiveOrderStations(orderHanger.OrderId, toStationType);

            List<SectionWrapper> toStationsWithNumberOfHangers = new List<SectionWrapper>();

            foreach (Section toStation in toStations)
            {
                toStationsWithNumberOfHangers.Add(new SectionWrapper()
                {
                    Section = toStation,
                    NumberOfHangersGoingToSection = orderHangerSectionServices.GetHangersGoingToSection(toStation).Count()
                });
            }

            toStationsWithNumberOfHangers.Sort((x, y) => x.NumberOfHangersGoingToSection.CompareTo(y.NumberOfHangersGoingToSection));

            foreach (SectionWrapper toStationWithNumberOfHangers in toStationsWithNumberOfHangers)
            {
                Section bufferBeforeStation = sectionServices.GetFirstPreviousSectionOfDevice(toStationWithNumberOfHangers.Section.InputDeviceId);
                int additionalBufferBeforeStationCapacity = 0;
                if (toStationType == StationTypeEnum.Buffer)
                {
                    Section additionalBufferBeforeStation = sectionServices.GetFirstPreviousSectionOfDevice(bufferBeforeStation.InputDeviceId);
                    additionalBufferBeforeStationCapacity = additionalBufferBeforeStation.Capacity;
                }
                if (toStationWithNumberOfHangers.NumberOfHangersGoingToSection < (toStationWithNumberOfHangers.Section.Capacity + bufferBeforeStation.Capacity + additionalBufferBeforeStationCapacity))
                {
                    List<Section> futurePath = pathServices.GetPath(from, toStationWithNumberOfHangers.Section);
                    orderHangerSectionServices.InsertFuturePath(orderHangerId, futurePath);
                    break;
                }
            }
        }
    }

    internal class SectionWrapper
    {
        internal Section Section { get; set; }
        internal int NumberOfHangersGoingToSection { get; set; }
    }
}
