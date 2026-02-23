using GlobalVidhanSabha.Helpers;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace GlobalVidhanSabha.Models.AdminMain
{
    public class AdminService : GlobalExceptionHandler.BaseService, IAdminService
    {
        private readonly string conn;

        public AdminService()
        {
            conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        // CREATE / UPDATE
        public async Task<int> SaveDesignationAsync(designationMain model)
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_DesignationType_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", model.DesignationId > 0 ? "updateDesignation" : "addDesignation");
                    cmd.Parameters.AddWithValue("@DesignationId", model.DesignationId);
                    cmd.Parameters.AddWithValue("@DesignationName", model.DesignationName);
                    //cmd.Parameters.AddWithValue("@DesignationType", model.DesignationType);

                    await con.OpenAsync();
                    object result = await cmd.ExecuteScalarAsync();

                    if (model.DesignationId == 0)
                    {
                        return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                    }
                    return model.DesignationId;
                }
            });
        }

        // DELETE
        public async Task<bool> DeleteDesignationAsync(int designationId)
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_DesignationType_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "DELETE");
                    cmd.Parameters.AddWithValue("@DesignationId", designationId);

                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
            });
        }

        // GET BY ID
        public async Task<designationMain> GetDesignationByIdAsync(int designationId)
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_DesignationType_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GETBYID");
                    cmd.Parameters.AddWithValue("@DesignationId", designationId);

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (!await dr.ReadAsync())
                            return null;

                        return new designationMain
                        {
                            DesignationId = dr.GetInt32Safe("DesignationId"),
                            DesignationName = dr.GetStringSafe("DesignationName"),
                            DesignationType = dr.GetStringSafe("DesignationType"),
                            Status = dr.GetBoolSafe("Status"),
                            CreatedDate = dr.GetDateTimeSafe("CreatedDate") ?? DateTime.MinValue,
                            UpdateDate = dr.GetDateTimeSafe("UpdateDate")
                        };
                    }
                }
            });
        }

        // GET ALL (PAGED)
        public async Task<PagedResult<designationMain>> GetAllDesignationsAsync(Pagination paging)
        {
            return await ExecuteAsync(async () =>
            {
                var list = new List<designationMain>();
                int totalRecords = 0;

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_DesignationType_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GETALL");
                    cmd.Parameters.AddWithValue("@PageNumber", paging.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", paging.Items);
                    cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(paging.search) ? null : paging.search);

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                            totalRecords = dr.GetInt32Safe("TotalRecords");

                        await dr.NextResultAsync(); 
                        while (await dr.ReadAsync())
                        {
                            list.Add(new designationMain
                            {
                                DesignationId = dr.GetInt32Safe("DesignationId"),
                                DesignationName = dr.GetStringSafe("DesignationName"),
                                //DesignationType = dr.GetStringSafe("designationType")
                            });
                        }
                    }
                }

                return new PagedResult<designationMain>
                {
                    data = list,
                    totalRecords = totalRecords
                };
            });
        }

        // ===== State Counts =====
        public async Task<int> SaveStateCountAsync(stateCountMain model)
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_UserStateCount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", model.Id == 0 ? "Add" : "Update");
                    if (model.Id > 0)
                        cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@StateId", model.StateId);
                    cmd.Parameters.AddWithValue("@Count", model.Count);

                    await con.OpenAsync();
                    object result = await cmd.ExecuteScalarAsync();

                    if (model.Id == 0)
                        return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                    return model.Id;
                }
            });
        }

        public async Task<bool> DeleteStateCountAsync(int StateId)
         {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_UserStateCount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "Delete");
                    cmd.Parameters.AddWithValue("@StateId", StateId);

                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
            });
        }

        public async Task<stateCountMain> GetStateCountByIdAsync(int id)
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_UserStateCount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetById");
                    cmd.Parameters.AddWithValue("@Id", id);

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (!await dr.ReadAsync())
                            return null;

                        return new stateCountMain
                        {
                            Id = dr.GetInt32Safe("Id"),
                            StateId = dr.GetInt32Safe("StateId"),
                            StateName = dr.GetStringSafe("StateName"),
                            Count = dr.GetInt32Safe("Count"),
                            CreatedDate = dr.GetDateTimeSafe("CreatedDate"),
                            UpdatedDate = dr.GetDateTimeSafe("UpdatedDate")
                        };
                    }
                }
            });
        }

        public async Task<PagedResult<stateCountMain>> GetAllStateCountsAsync(Pagination paging)
        {
            return await ExecuteAsync(async () =>
            {
                int totalRecords = 0;
                var list = new List<stateCountMain>();

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_UserStateCount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetAll");
                    cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = paging?.PageNumber ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = paging?.Items ?? (object)DBNull.Value;
                    cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(paging?.search) ? (object)DBNull.Value : paging.search);

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            if (totalRecords == 0)
                                totalRecords = dr.GetInt32Safe("TotalCount");

                            list.Add(new stateCountMain
                            {
                                Id = dr.GetInt32Safe("Id"),
                                StateId = dr.GetInt32Safe("StateId"),
                                StateName = dr.GetStringSafe("StateName"),
                                Count = dr.GetInt32Safe("Count"),
                                CreatedDate = dr.GetDateTimeSafe("CreatedDate"),
                                RemainingCount = dr.GetInt32Safe("RemainingCount")
                            });
                        }
                    }
                }

                return new PagedResult<stateCountMain>
                {
                    data = list,
                    totalRecords = totalRecords
                };
            });
        }

        public async Task<List<State>> GetAllStateAsync()
        {
            return await ExecuteAsync(async () =>
            {
                var list = new List<State>();
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_UserStateCount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetAllState");

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            list.Add(new State
                            {
                                StateId = dr.GetInt32Safe("StateId"),
                                StateName = dr.GetStringSafe("StateName")
                            });
                        }
                    }
                }
                return list;
            });
        }

        // ===== District Counts =====
        public async Task<int> SaveDistictAsync(DistrictCountModel model)
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_UserStateCount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", model.Id == 0 ? "addDistict" : "UpdateDistict");
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@StateId", model.StateId);
                    cmd.Parameters.AddWithValue("@DistrictId", model.DistrictId);
                    cmd.Parameters.AddWithValue("@Count", model.Count);

                    await con.OpenAsync();

                    if (model.Id == 0)
                    {
                        object result = await cmd.ExecuteScalarAsync();
                        return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                    }
                    else
                    {
                        await cmd.ExecuteNonQueryAsync();
                        return model.Id;
                    }
                }
            });
        }

        public async Task<bool> DeleteDistictAsync(int DistrictId)
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_UserStateCount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "DeleteDistict");
                    cmd.Parameters.AddWithValue("@DistrictId", DistrictId);

                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
            });
        }

        public async Task<DistrictCountModel> GetDistictByIdAsync(int id)
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_UserStateCount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetByIdDistict");
                    cmd.Parameters.AddWithValue("@Id", id);

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (!await dr.ReadAsync())
                            return null;

                        return new DistrictCountModel
                        {
                            Id = dr.GetInt32Safe("Id"),
                            StateId = dr.GetInt32Safe("StateId"),
                            DistrictId = dr.GetInt32Safe("DistrictId"),
                            Count = dr.GetInt32Safe("Count")
                        };
                    }
                }
            });
        }

        public async Task<PagedResult<DistrictCountModel>> GetAllDistictAsync(Pagination paging)
        {
            return await ExecuteAsync(async () =>
            {
                int totalRecords = 0;
                var list = new List<DistrictCountModel>();

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_UserStateCount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetAllDistict");
                    cmd.Parameters.AddWithValue("@PageNumber", paging.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", paging.Items);
                    cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(paging.search) ? null : paging.search);

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            if (totalRecords == 0)
                                totalRecords = dr.GetInt32Safe("TotalCount");

                            list.Add(new DistrictCountModel
                            {
                                Id = dr.GetInt32Safe("Id"),
                                StateId = dr.GetInt32Safe("StateId"),
                                DistrictId = dr.GetInt32Safe("DistrictId"),
                                DistrictName = dr.GetStringSafe("DistrictName"),
                                StateName = dr.GetStringSafe("StateName"),
                                Count = dr.GetInt32Safe("Count"),
                                RemainingCount = dr.GetInt32Safe("RemainingCount")
                            });
                        }
                    }
                }

                return new PagedResult<DistrictCountModel>
                {
                    data = list,
                    totalRecords = totalRecords
                };
            });
        }

        public async Task<List<DistrictModel>> GetDistrictsByStateAsync(int stateId)
        {
            return await ExecuteAsync(async () =>
            {
                var list = new List<DistrictModel>();
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_UserStateCount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetDistrictsByStateId");
                    cmd.Parameters.AddWithValue("@StateId", stateId);

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            list.Add(new DistrictModel
                            {
                                DistrictId = dr.GetInt32Safe("DistrictId"),
                                DistrictName = dr.GetStringSafe("DistrictName")
                            });
                        }
                    }
                }
                return list;
            });
        }

        // ===== Vidhan Sabha Registration =====
        public async Task<int> SaveVidhanSabhaRegistrationAsync(VidhanSabhaRegister model)
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_DesignationType_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    string generatedPassword = null;
                    if (model.Prabhari)
                    {
                        generatedPassword = PasswordGenerator.GeneratePassword(model.Name, model.PhoneNo);
                    }

                    cmd.Parameters.AddWithValue("@Action", model.Id > 0 ? "UpdateVidhanSabhaRagitration" : "VidhanSabhaRagitration");
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@VidhanSabhaName", model.VidhanSabhaName);
                    cmd.Parameters.AddWithValue("@Prabhari", model.Prabhari);
                    cmd.Parameters.AddWithValue("@Name", (object)model.Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)model.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNo", (object)model.PhoneNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Category", (object)model.Category ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Caste", (object)model.Caste ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StateId", model.StateId);
                    cmd.Parameters.AddWithValue("@DistrictId", model.DistrictId);
                    cmd.Parameters.AddWithValue("@Profile", (object)model.Profile ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Education", (object)model.Education ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)model.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Profession", (object)model.Profession ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserName", (object)model.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", (object)generatedPassword ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Role", "VidhanSabhaPrabhari");

                    await con.OpenAsync();
                    object result = await cmd.ExecuteScalarAsync();
                    int newId = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;

                    if (model.Prabhari && newId > 0)
                    {
                        await EmailService.SendLoginEmailAsync(model.Email, model.PhoneNo, generatedPassword);
                    }

                    return newId;
                }
            });
        }

        public async Task<List<CasteCategory>> GetCasteCategoryAsync()
        {
            return await ExecuteAsync(async () =>
            {
                var list = new List<CasteCategory>();
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_DesignationType_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetCasteCategory");

                    await con.OpenAsync();
                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            list.Add(new CasteCategory
                            {
                                Id = dr.GetInt32Safe("Id"),
                                Name = dr.GetStringSafe("CategoryName")
                            });
                        }
                    }
                }
                return list;
            });
        }

        public async Task<List<CasteCategory>> GetSubCasteByCategoryAsync(int categoryId)
        {
            return await ExecuteAsync(async () =>
            {
                var list = new List<CasteCategory>();
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_DesignationType_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetSubCasteByCategory");
                    cmd.Parameters.AddWithValue("@categoryId", categoryId);

                    await con.OpenAsync();
                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            list.Add(new CasteCategory
                            {
                                Id = dr.GetInt32Safe("Id"),
                                Name = dr.GetStringSafe("SubCasteName")
                            });
                        }
                    }
                }
                return list;
            });
        }

        public async Task<PagedResult<VidhanSabhaRegister>> GetAllVidhanSabhaAsync(Pagination paging, bool? Prabhari = null, int? stateId = null)
        {
            return await ExecuteAsync(async () =>
            {
                int totalRecords = 0;
                var list = new List<VidhanSabhaRegister>();

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_DesignationType_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "getAllVidhanSabha");
                    cmd.Parameters.AddWithValue("@PageNumber", paging.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", paging.Items);
                    cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(paging.search) ? null : paging.search);
                    cmd.Parameters.Add("@StateId", SqlDbType.Int).Value = (object)stateId ?? DBNull.Value;
                    cmd.Parameters.Add("@Prabhari", SqlDbType.Bit).Value = (object)Prabhari ?? DBNull.Value;

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            if (totalRecords == 0)
                                totalRecords = dr.GetInt32Safe("TotalCount");

                            list.Add(new VidhanSabhaRegister
                            {
                                Id = dr.GetInt32Safe("Id"),
                                StateId = dr.GetNullableInt32Safe("StateId"),
                                DistrictId = dr.GetNullableInt32Safe("DistrictId"),
                                VidhanSabhaName = dr.GetStringSafe("VidhanSabhaName"),
                                Prabhari = dr.GetBoolSafe("Prabhari"),
                                Name = dr.GetStringSafe("Name"),
                                Email = dr.GetStringSafe("Email"),
                                PhoneNo = dr.GetStringSafe("PhoneNo"),
                                Category = dr.GetNullableInt32Safe("Category"),
                                Caste = dr.GetNullableInt32Safe("Caste"),
                                Profile = dr.GetStringSafe("Profile"),
                                Education = dr.GetStringSafe("Education"),
                                Address = dr.GetStringSafe("Address"),
                                Profession = dr.GetStringSafe("Profession"),
                                CreatedAt = dr.GetDateTimeSafe("CreatedAt") ?? DateTime.MinValue,
                                UpdatedAt = dr.GetDateTimeSafe("UpdatedAt"),
                                Status = dr.GetBoolSafe("Status"),
                                DistrictName = dr.GetStringSafe("DistrictName"),
                                StateName = dr.GetStringSafe("StateName")
                            });
                        }
                    }
                }

                return new PagedResult<VidhanSabhaRegister>
                {
                    data = list,
                    totalRecords = totalRecords
                };
            });
        }

        public async Task<VidhanSabhaRegister> GetVidhanSabhaByIdAsync(int id)
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_DesignationType_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetVidhanSabhaById");
                    cmd.Parameters.AddWithValue("@Id", id);

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (!await dr.ReadAsync())
                            return null;

                        return new VidhanSabhaRegister
                        {
                            Id = dr.GetInt32Safe("Id"),
                            VidhanSabhaName = dr.GetStringSafe("VidhanSabhaName"),
                            Prabhari = dr.GetBoolSafe("Prabhari"),
                            Name = dr.GetStringSafe("Name"),
                            Email = dr.GetStringSafe("Email"),
                            PhoneNo = dr.GetStringSafe("PhoneNo"),
                            StateId = dr.GetNullableInt32Safe("StateId"),
                            DistrictId = dr.GetNullableInt32Safe("DistrictId"),
                            StateName = dr.GetStringSafe("StateName"),
                            DistrictName = dr.GetStringSafe("DistrictName"),
                            Category = dr.GetNullableInt32Safe("Category"),
                            CategoryName = dr.GetStringSafe("CategoryName"),
                            Caste = dr.GetNullableInt32Safe("Caste"),
                            SubCasteName = dr.GetStringSafe("SubCasteName"),
                            Profile = dr.GetStringSafe("UserProfile"),
                            Education = dr.GetStringSafe("Education"),
                            Address = dr.GetStringSafe("Address"),
                            Profession = dr.GetStringSafe("Profession"),
                            CreatedAt = dr.GetDateTimeSafe("CreatedAt") ?? DateTime.MinValue,
                            UpdatedAt = dr.GetDateTimeSafe("UpdatedAt"),
                            Status = dr.GetBoolSafe("Status")
                        };
                    }
                }
            });
        }

        public async Task<bool> DeleteVidhanSabhaAsync(int id)
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_DesignationType_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "DeleteVidhanSabha");
                    cmd.Parameters.AddWithValue("@Id", id);

                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
            });
        }

        public async Task<List<DistrictCountModel>> GetAllDistrictsDataByStateId(int stateId)
        {
            return await ExecuteAsync(async () =>
            {
                var districts = new List<DistrictCountModel>();
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_UserStateCount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetAllDistrictsDataByStateId");
                    cmd.Parameters.AddWithValue("@StateId", stateId);

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            districts.Add(new DistrictCountModel
                            {
                                Id = dr.GetInt32Safe("Id"),
                                StateId = dr.GetInt32Safe("StateId"),
                                DistrictId = dr.GetInt32Safe("DistrictId"),
                                DistrictName = dr.GetStringSafe("DistrictName"),
                                StateName = dr.GetStringSafe("StateName"),
                                Count = dr.GetInt32Safe("Count"),
                                RemainingCount = dr.GetInt32Safe("RemainingCount")
                            });
                        }
                    }
                }
                return districts;
            });
        }

        public async Task<Dashboard> GetDashboardCountsAsync()
        {
            return await ExecuteAsync(async () =>
            {
                var dashboard = new Dashboard();
                using (SqlConnection con = new SqlConnection(conn))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("usp_Dashboard", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", "TotalStatePrabhari");
                        object result = await cmd.ExecuteScalarAsync();
                        dashboard.TotalStatePrabhari = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                    }

                    using (SqlCommand cmd = new SqlCommand("usp_Dashboard", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", "TotalVidhanSabhaWithoutPrabhari");
                        object result = await cmd.ExecuteScalarAsync();
                        dashboard.TotalVidhanSabhaWithoutPrabhari = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                    }

                    using (SqlCommand cmd = new SqlCommand("usp_Dashboard", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", "TotalUsedStates");
                        object result = await cmd.ExecuteScalarAsync();
                        dashboard.TotalUsedStates = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                    }

                    using (SqlCommand cmd = new SqlCommand("usp_Dashboard", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", "TotalUsedDistrict");
                        object result = await cmd.ExecuteScalarAsync();
                        dashboard.TotalUsedDistrict = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                    }
                    using (SqlCommand cmd = new SqlCommand("usp_Dashboard", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", "TotalDesignationName");
                        object result = await cmd.ExecuteScalarAsync();
                        dashboard.TotalDesignationName = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                    }
                }
                return dashboard;
            });
        }

        public async Task<PagedResult<VidhanSabhaRegister>> GetVidhanSabhaByStateIdAsync(int DistrictId, Pagination paging)
        {
            return await ExecuteAsync(async () =>
            {
                int totalRecords = 0;
                var list = new List<VidhanSabhaRegister>();

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_DesignationType_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetVidhanSabhaByStateId");
                    cmd.Parameters.AddWithValue("@DistrictId", DistrictId);
                    cmd.Parameters.AddWithValue("@PageNumber", paging.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", paging.Items);
                    cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(paging.search) ? null : paging.search);

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            if (totalRecords == 0)
                                totalRecords = dr.GetInt32Safe("TotalCount");

                            list.Add(new VidhanSabhaRegister
                            {
                                Id = dr.GetInt32Safe("Id"),
                                VidhanSabhaName = dr.GetStringSafe("VidhanSabhaName"),
                                Prabhari = dr.GetBoolSafe("Prabhari"),
                                Name = dr.GetStringSafe("Name"),
                                Email = dr.GetStringSafe("Email"),
                                PhoneNo = dr.GetStringSafe("PhoneNo"),
                                Category = dr.GetNullableInt32Safe("Category"),
                                Caste = dr.GetNullableInt32Safe("Caste"),
                                Profile = dr.GetStringSafe("Profile"),
                                Education = dr.GetStringSafe("Education"),
                                Address = dr.GetStringSafe("Address"),
                                Profession = dr.GetStringSafe("Profession"),
                                CreatedAt = dr.GetDateTimeSafe("CreatedAt") ?? DateTime.MinValue,
                                UpdatedAt = dr.GetDateTimeSafe("UpdatedAt"),
                                Status = dr.GetBoolSafe("Status"),
                                DistrictName = dr.GetStringSafe("DistrictName"),
                                StateName = dr.GetStringSafe("StateName")
                            });
                        }
                    }
                }

                return new PagedResult<VidhanSabhaRegister>
                {
                    data = list,
                    totalRecords = totalRecords
                };
            });
        }

        public async Task<List<KeyValuePair<string, int>>> GetStateWiseVidhanSabhaChartAsync()
        {
            return await ExecuteAsync(async () =>
            {
                var list = new List<KeyValuePair<string, int>>();
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_Dashboard", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "ChartDataGetStateWiseVidhanSabha");

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            list.Add(new KeyValuePair<string, int>(
                                dr.GetStringSafe("StateName"),
                                dr.GetInt32Safe("TotalVidhanSabha")
                            ));
                        }
                    }
                }
                return list;
            });
        }

        public async Task<List<KeyValuePair<string, int>>> GetDistrictWiseVidhanSabhaChartAsync(int? StateId)
        {
            return await ExecuteAsync(async () =>
            {
                var list = new List<KeyValuePair<string, int>>();
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_Dashboard", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "ChartDataGetDistrictWiseVidhanSabha");
                    cmd.Parameters.AddWithValue("@StateId", StateId);

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            list.Add(new KeyValuePair<string, int>(
                                dr.GetStringSafe("DistrictName"),
                                dr.GetInt32Safe("TotalVidhanSabha")
                            ));
                        }
                    }
                }
                return list;
            });
        }

        public async Task<List<designationType>> GetDesignationTypeAsync()
        {
            return await ExecuteAsync(async () =>
            {
                var list = new List<designationType>();
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("usp_DesignationType_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetdesignationType");

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            list.Add(new designationType
                            {
                                DesignationId = dr.GetInt32Safe("DesignationId"),
                                DesignationType = dr.GetStringSafe("DesignationType")
                            });
                        }
                    }
                }
                return list;
            });
        }

        // ===== State Prabhari =====
        public async Task<PagedResult<StatePrabhariModel>> GetAllStatePrabhariAsync(Pagination paging)
        {
            return await ExecuteAsync(async () =>
            {
                int totalRecords = 0;
                var list = new List<StatePrabhariModel>();

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("sp_StatePrabhari", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GETALLSTATEPRABHARI");
                    cmd.Parameters.AddWithValue("@PageNumber", paging.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", paging.Items);
                    cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(paging.search) ? (object)DBNull.Value : paging.search);

                    await con.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            if (totalRecords == 0)
                                totalRecords = dr.GetInt32Safe("TotalCount");

                            list.Add(new StatePrabhariModel
                            {
                                Id = dr.GetInt32Safe("Id"),
                                State = dr.GetNullableInt32Safe("state"),
                                StateName = dr.GetStringSafe("StateName"),
                                PrabhariName = dr.GetStringSafe("PrabhariName"),
                                Email = dr.GetStringSafe("Email"),
                                PhoneNo = dr.GetStringSafe("PhoneNo"),
                                Category = dr.GetNullableInt32Safe("Category"),
                                CategoryName = dr.GetStringSafe("CategoryName"),
                                SubCaste = dr.GetNullableInt32Safe("SubCaste"),
                                SubCasteName = dr.GetStringSafe("SubCasteName"),
                                Address = dr.GetStringSafe("Address"),
                                Profession = dr.GetStringSafe("Profession"),
                                Education = dr.GetStringSafe("Education")
                            });
                        }
                    }
                }

                return new PagedResult<StatePrabhariModel>
                {
                    data = list,
                    totalRecords = totalRecords
                };
            });
        }

        public async Task<StatePrabhariModel> GetStatePrabhariByIdAsync(int id)
        {
            return await ExecuteAsync(async () =>
            {
                StatePrabhariModel model = null;
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("sp_StatePrabhari", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GETBYID");
                    cmd.Parameters.AddWithValue("@Id", id);

                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            model = new StatePrabhariModel
                            {
                                Id = reader.GetInt32Safe("Id"),
                                State = reader.GetNullableInt32Safe("State"),
                                PrabhariName = reader.GetStringSafe("PrabhariName"),
                                Email = reader.GetStringSafe("Email"),
                                PhoneNo = reader.GetStringSafe("PhoneNo"),
                                Category = reader.GetNullableInt32Safe("Category"),
                                SubCaste = reader.GetNullableInt32Safe("SubCaste"),
                                Education = reader.GetStringSafe("Education"),
                                Profession = reader.GetStringSafe("Profession"),
                                Profile = reader.GetStringSafe("Profile"),
                                Address = reader.GetStringSafe("Address")
                            };
                        }
                    }
                }
                return model;
            });
        }

        public async Task<int> SaveStatePrabhariAsync(StatePrabhariModel model)
        
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("sp_StatePrabhari", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    string generatedPassword = null;
                    bool isNew = model.Id == 0;

                    if (isNew)
                    {
                        generatedPassword = PasswordGenerator.GeneratePassword(model.PrabhariName, model.PhoneNo);
                    }

                    cmd.Parameters.AddWithValue("@Action", isNew ? "ADD" : "UPDATE");
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@State", (object)model.State ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrabhariName", (object)model.PrabhariName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)model.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNo", (object)model.PhoneNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Category", (object)model.Category ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubCaste", (object)model.SubCaste ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Education", (object)model.Education ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Profession", (object)model.Profession ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Profile", (object)model.Profile ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)model.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserName", (object)model.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", (object)generatedPassword ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Role", "StatePrabhari");

                    await con.OpenAsync();
                    object result = await cmd.ExecuteScalarAsync();
                    int newId = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;

                    if (isNew && newId > 0)
                    {
                        await EmailService.SendLoginEmailAsync(model.Email, model.PhoneNo, generatedPassword);
                    }

                    return newId;
                }
            });
        }

        public async Task<bool> DeleteStatePrabhariAsync(int id)
        {
            return await ExecuteAsync(async () =>
            {
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("sp_StatePrabhari", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "DELETE");
                    cmd.Parameters.AddWithValue("@Id", id);

                    await con.OpenAsync();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            });
        }
    }
}