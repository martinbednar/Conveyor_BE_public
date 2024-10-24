using DAL.Data;
using DAL.Models;

namespace BL.Services
{
    public class SectionServices
    {
        MyDbContext dbContext = new();

        public List<Section> GetPreviousSectionsOfDevice(int deviceId)
        {
            return dbContext.Sections.Where(s => s.OutputDeviceId == deviceId).ToList();
        }

        public List<Section> GetNextSectionsOfDevice(int deviceId)
        {
            return dbContext.Sections.Where(s => s.InputDeviceId == deviceId).ToList();
        }

        public Section GetFirstPreviousSectionOfDevice(int deviceId)
        {
            return GetPreviousSectionsOfDevice(deviceId).First();
        }

        public Section GetFirstNextSectionOfDevice(int deviceId)
        {
            return GetNextSectionsOfDevice(deviceId).First();
        }

        public Section GetPreviousSectionOfDeviceFromDirect(int deviceId, SectionDirectionEnum direct)
        {
            return GetPreviousSectionsOfDevice(deviceId).Where(s => s.SectionDirectionToOutputDeviceId == (int)direct).First();
        }

        public Section GetNextSectionOfDeviceToDirect(int deviceId, SectionDirectionEnum direct)
        {
            return GetNextSectionsOfDevice(deviceId).Where(s => s.SectionDirectionFromInputDeviceId == (int)direct).First();
        }

        public bool SectionIsNotFull(Section section)
        {
            OrderHangerSectionServices orderHangerSectionServices = new OrderHangerSectionServices();
            return section.Capacity > orderHangerSectionServices.GetHangersInSection(section).Count;
        }

        public Section GetSectionById(int sectionId)
        {
            return dbContext.Sections.Find(sectionId);
        }

        internal List<Section> GetAllSections()
        {
            return dbContext.Sections.ToList();
        }
    }
}
