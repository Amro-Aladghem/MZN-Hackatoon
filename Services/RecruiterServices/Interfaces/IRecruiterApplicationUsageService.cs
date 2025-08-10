using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.RecruiterServices.Interfaces
{
    public interface IRecruiterApplicationUsageService
    {
        public Task<int> GetNumberOfAvailableTaskForRecruiter(int RecruiterId);
        public Task<bool> DecreaseTheAvailableApplicationsInAccount(int RecruiterId);
        public Task<bool> IsRecruiterHasAvailableApplicationsInAccount(int RecruiterId);

        public Task<bool> AddInitialRecruiterApplicationUsageInfo(int RecruiterId);
    }
}
