using DocumentFormat.OpenXml.EMMA;
using GlobalVidhanSabha.Helpers;
using GlobalVidhanSabha.Models.AdminMain;
using GlobalVidhanSabha.Models.SamithiMember;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static GlobalVidhanSabha.Models.SamithiMember.VidhanSabhaModel;

public class SamithiMemberService : GlobalExceptionHandler.BaseService, ISamithiMemberService
{
    private readonly string conn;
    private SqlConnection con;

    public SamithiMemberService()
    {
        conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    }
    // ADD THIS METHOD HERE

    public async Task<int> SaveMemberAsync(SamithiMemberModel member)
    {
        return await ExecuteAsync(async () =>
        {
            using (SqlConnection con = new SqlConnection(conn))
            using (SqlCommand cmd = new SqlCommand("sp_ManageSamithiMember", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                string action = member.Id.HasValue && member.Id > 0 ? "UPDATE" : "ADD";

                cmd.Parameters.AddWithValue("@Action", action);

                if (action == "UPDATE")
                    cmd.Parameters.AddWithValue("@Id", member.Id.Value);

                cmd.Parameters.AddWithValue("@DesignationType", (object)member.DesignationType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DesignationNameId", (object)member.DesignationNameId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SamithiMember", (object)member.SamithiMember ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)member.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNo", (object)member.PhoneNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Category", (object)member.Category ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Caste", (object)member.Caste ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Education", (object)member.Education ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Profession", (object)member.Profession ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object)member.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProfilePath", (object)member.ProfilePath ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@stateId", member.stateId);

                await con.OpenAsync();

                return await cmd.ExecuteNonQueryAsync();
            }
        });
    }


    public async Task<int> DeleteMemberAsync(int id)
    {
        return await ExecuteAsync(async () =>
        {
            using (SqlConnection con = new SqlConnection(conn))
            using (SqlCommand cmd = new SqlCommand("sp_ManageSamithiMember", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@Id", id);

                await con.OpenAsync();
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows;
            }
        });
       
    }

    public async Task<List<SamithiMemberModel>> GetAllMembersAsync(int? stateId, Pagination paging)
    {
        return await ExecuteAsync(async () =>
        {
            var members = new List<SamithiMemberModel>();

            using (SqlConnection con = new SqlConnection(conn))
            using (SqlCommand cmd = new SqlCommand("sp_ManageSamithiMember", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETALL");
                cmd.Parameters.AddWithValue("@PageNumber", paging.PageNumber);
                cmd.Parameters.AddWithValue("@PageSize", paging.Items);
                cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(paging.search) ? null : paging.search);
                cmd.Parameters.AddWithValue("@stateId",
              stateId.HasValue ? (object)stateId.Value : DBNull.Value);

                await con.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        members.Add(new SamithiMemberModel
                        {
                            Id = reader["Id"] as int?,
                            DesignationType = reader["DesignationType"] as int?,
                            DesignationTypeName = reader["DesignationTypeName"].ToString(),
                            DesignationName = reader["DesignationName"].ToString(),
                            SamithiMember = reader["SamithiMember"].ToString(),
                            Email = reader["Email"].ToString(),
                            PhoneNo = reader["PhoneNo"].ToString(),
                            Category = reader["Category"] as int?,
                            Caste = reader["Caste"] as int?,
                            Education = reader["Education"].ToString(),
                            Profession = reader["Profession"].ToString(),
                            Address = reader["Address"].ToString(),
                            ProfilePath = reader["ProfilePath"].ToString(),
                            stateId = reader["stateId"] as int?
                        });
                    }
                }
            }
            return members;
        });
        
    }

    public async Task<SamithiMemberModel> GetMemberByIdAsync(int id)
    {
        return await ExecuteAsync(async () =>
        {
            SamithiMemberModel member = null;

            using (SqlConnection con = new SqlConnection(conn))
            using (SqlCommand cmd = new SqlCommand("sp_ManageSamithiMember", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETBYID");
                cmd.Parameters.AddWithValue("@Id", id);

                await con.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        member = new SamithiMemberModel
                        {
                            Id = reader["Id"] as int?,
                            DesignationType = reader["DesignationType"] as int?,
                            SamithiMember = reader["SamithiMember"].ToString(),
                            DesignationName= reader["DesignationName"].ToString(),
                            Email = reader["Email"].ToString(),
                            PhoneNo = reader["PhoneNo"].ToString(),
                            Category = reader["Category"] as int?,
                            Caste = reader["Caste"] as int?,
                            Education = reader["Education"].ToString(),
                            Profession = reader["Profession"].ToString(),
                            Address = reader["Address"].ToString(),
                            ProfilePath = reader["ProfilePath"].ToString()
                        };
                    }
                }
            }
            return member;
        });
        
    }

    public async Task<MemberDashboardCount> GetDashboardCountAsync(int? StateId)
    {
        return await ExecuteAsync(async () =>
        {
            MemberDashboardCount dashboard = new MemberDashboardCount();


            using (SqlConnection con = new SqlConnection(conn))
            {
                await con.OpenAsync();

                // Total VidhanSabha
                using (SqlCommand cmd2 = new SqlCommand("usp_Dashboard", con))
                {
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Action", "TotalVidhanSabhaCount");
                    cmd2.Parameters.AddWithValue("@StateId", (object)StateId ?? DBNull.Value);

                    var result = await cmd2.ExecuteScalarAsync();
                    dashboard.TotalVidhanSabhaCount = result != null ? Convert.ToInt32(result) : 0;
                }

                // With Prabhari
                using (SqlCommand cmd3 = new SqlCommand("usp_Dashboard", con))
                {
                    cmd3.CommandType = CommandType.StoredProcedure;
                    cmd3.Parameters.AddWithValue("@Action", "TotalVidhanSabhaWithoutPrabhari");
                    cmd3.Parameters.AddWithValue("@StateId", (object)StateId ?? DBNull.Value);

                    var result = await cmd3.ExecuteScalarAsync();
                    dashboard.TotalVidhanSabhaWithoutPrabhari = result != null ? Convert.ToInt32(result) : 0;
                }

                // WithOut Prabhari
                using (SqlCommand cmd3 = new SqlCommand("usp_Dashboard", con))
                {
                    cmd3.CommandType = CommandType.StoredProcedure;
                    cmd3.Parameters.AddWithValue("@Action", "TotalVidhanSabhaWithPrabhari");
                    cmd3.Parameters.AddWithValue("@StateId", (object)StateId ?? DBNull.Value);

                    var result = await cmd3.ExecuteScalarAsync();
                    dashboard.TotalVidhanSabhaWithPrabhari = result != null ? Convert.ToInt32(result) : 0;
                }
                //Total Count Samithi Mmenber
                using (SqlCommand cmd3 = new SqlCommand("usp_Dashboard", con))
                {
                    cmd3.CommandType = CommandType.StoredProcedure;
                    cmd3.Parameters.AddWithValue("@Action", "TotalCountSamithiMmenber");
                    cmd3.Parameters.AddWithValue("@StateId", (object)StateId ?? DBNull.Value);

                    var result = await cmd3.ExecuteScalarAsync();
                    dashboard.TotalCountSamithiMmenber = result != null ? Convert.ToInt32(result) : 0;
                }


            }

            return dashboard;
        });
    }


}

