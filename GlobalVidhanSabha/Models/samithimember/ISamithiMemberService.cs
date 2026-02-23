using GlobalVidhanSabha.Models.AdminMain;
using System.Collections.Generic;
using System.Threading.Tasks;
using static GlobalVidhanSabha.Models.SamithiMember.VidhanSabhaModel;

namespace GlobalVidhanSabha.Models.SamithiMember
{
    public interface ISamithiMemberService
    {

        Task<int> SaveMemberAsync(SamithiMemberModel member);
        Task<int> DeleteMemberAsync(int id);
        Task<List<SamithiMemberModel>> GetAllMembersAsync(int? stateId, Pagination paging);
        Task<SamithiMemberModel> GetMemberByIdAsync(int id);

        Task<MemberDashboardCount> GetDashboardCountAsync(int? stateId);
    }
}
