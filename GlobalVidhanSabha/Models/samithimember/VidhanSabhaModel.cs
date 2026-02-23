using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalVidhanSabha.Models.SamithiMember
{
    public class VidhanSabhaModel
    {
        public class SamithiMemberModel
        {
            public int? Id { get; set; }
            public int? stateId { get; set; }
            public int? DesignationType { get; set; }
            public int? DesignationNameId { get; set; }

            public string DesignationTypeName { get; set; }
            public string DesignationName { get; set; }

            public string SamithiMember { get; set; }
            public string Email { get; set; }
            public string PhoneNo { get; set; }
            public int? Category { get; set; }
            public int? Caste { get; set; }
            public string Education { get; set; }
            public string Profession { get; set; }
            public string Address { get; set; }
            public string ProfilePath { get; set; }
        }

        public class MemberDashboardCount
        {
            public int TotalMembers { get; set; }

            public int TotalVidhanSabhaCount { get; set; }
            public int TotalVidhanSabhaWithPrabhari { get; set; }
            public int  TotalVidhanSabhaWithoutPrabhari { get; set; }
            public int TotalCountSamithiMmenber { get; set; }
        }

    }
}