using DocumentFormat.OpenXml.EMMA;
using GlobalVidhanSabha.Helpers;
using GlobalVidhanSabha.Models.AdminMain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using GlobalVidhanSabha.Helpers;
using VishanSabha.Services;

namespace GlobalVidhanSabha.vidhanSabhaApiController
{
    [RoutePrefix("api/Admin")]
    public class MasterApiController : BaseApiController
    {
        private readonly IAdminService _service;


        public MasterApiController()
        {
            _service = new AdminService();

        }

        private int StateId
        {
            get
            {
                return HttpContext.Current?.Session?["StateId"] != null
                    ? Convert.ToInt32(HttpContext.Current.Session["StateId"])
                    : 0;
            }
        }

        [HttpGet]
        [Route("getAllDesignation")]
        public async Task<IHttpActionResult> GetAll([FromUri] Pagination paging)
        {
            return await ProcessRequestAsync(async () =>
            {
                var list = await _service.GetAllDesignationsAsync(paging);
                return list;
            });
        }

        [HttpGet]
        [Route("getDesignationByid")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetDesignationByIdAsync(id);
            });
        }


        [HttpPost]
        [Route("SaveDesignation")]
        public async Task<IHttpActionResult> SaveDesignation([FromBody] designationMain model)
        {
            if (model == null)
                return ApiBadRequest("Invalid designation data");

            bool isUpdate = model.DesignationId > 0;

            return await ProcessCreateOrUpdateAsync(async () =>
            {
                int id = await _service.SaveDesignationAsync(model);
                return id > 0 ? id : (int?)null;
            }, isUpdate);
        }

        [HttpDelete]
        [Route("DeleteDesignation")]
        public async Task<IHttpActionResult> DeleteDesignation(int id)
        {
            return await ProcessDeleteAsync(async () =>
            {
                return await _service.DeleteDesignationAsync(id);
            });
        }




        //=========================================================================

        [HttpGet]
        [Route("getAllStateCount")]
        public async Task<IHttpActionResult> GetAllStateCount([FromUri] Pagination paging = null)
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetAllStateCountsAsync(paging);
            });
        }

        [HttpGet]
        [Route("getByIdStateCount")]
        public async Task<IHttpActionResult> GetByIdStateCount(int id)
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetStateCountByIdAsync(id);
            });
        }

        [HttpPost]
        [Route("saveStateCount")]
        public async Task<IHttpActionResult> Save([FromBody] stateCountMain model)
        {
            if (model == null)
                return ApiBadRequest("Invalid data");

            bool isUpdate = model.Id > 0;

            return await ProcessCreateOrUpdateAsync(async () =>
            {
                int id = await _service.SaveStateCountAsync(model);
                return id > 0 ? id : (int?)null;
            }, isUpdate);
        }

        [HttpDelete]
        [Route("deleteStateCount")]
        public async Task<IHttpActionResult> Delete(int StateId)
        {
            return await ProcessDeleteAsync(async () =>
            {
                return await _service.DeleteStateCountAsync(StateId);
            });
        }

        [HttpGet]
        [Route("getAllState")]
        public async Task<IHttpActionResult> GetAllState()
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetAllStateAsync();
            });
        }
        //=====================

        [HttpPost]
        [Route("SaveDistict")]
        public async Task<IHttpActionResult> SaveDistict([FromBody] DistrictCountModel model)
        {
            if (model == null)
                return ApiBadRequest("Invalid District data");

          
                int id = await _service.SaveDistictAsync(model);

                return Ok(new
                {
                    success = true,
                    message = model.Id == 0 ? "District added successfully!" : "District updated successfully!",
                    data = id
                });
                                              
        }


        [HttpDelete]
        [Route("deleteDistict")]
        public async Task<IHttpActionResult> DeleteDistict(int DistrictId)
        {
            return await ProcessDeleteAsync(async () =>
            {
                return await _service.DeleteDistictAsync(DistrictId);
            });
        }

        [HttpGet]
        [Route("getDistictById")]
        public async Task<IHttpActionResult> GetDistictById(int id)
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetDistictByIdAsync(id);
            });
        }

        [HttpGet]
        [Route("getAllDistict")]
        public async Task<IHttpActionResult> GetAllDistict([FromUri] Pagination paging)
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetAllDistictAsync(paging);
            });
        }



        [HttpGet]
        [Route("getDistrictsByState")]
        public async Task<IHttpActionResult> GetDistrictsByState(int stateId)
        {
            return await ProcessRequestAsync(async () =>
            {
                var districts = await _service.GetDistrictsByStateAsync(stateId);
                return new { success = true, data = districts };
            });
        }

        [HttpGet]
        [Route("GetCasteCategory")]
        public async Task<IHttpActionResult> GetCasteCategory()
        {
            return await ProcessRequestAsync(async () =>
            {
                var data = await _service.GetCasteCategoryAsync();
                return new { success = true, data = data };
            });
        }

        [HttpGet]
        [Route("GetSubCasteByCategory")]
        public async Task<IHttpActionResult> GetSubCasteByCategory(int categoryId)
        {
            return await ProcessRequestAsync(async () =>
            {
                var data = await _service.GetSubCasteByCategoryAsync(categoryId);
                return new { success = true, data = data };
            });


        }



        [HttpGet]
        [Route("getAllVidhanSabha")]
        public async Task<IHttpActionResult> GetAllVidhanSabha([FromUri] Pagination paging,[FromUri] string filter = null)
        {
            return await ProcessRequestAsync(async () =>
            {
                bool? prabhari = null;

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    if (filter.Equals("withPrabhari", StringComparison.OrdinalIgnoreCase))
                        prabhari = true;
                    else if (filter.Equals("withoutPrabhari", StringComparison.OrdinalIgnoreCase))
                        prabhari = false;
                }
                //int? stateId = null;

                //if (HttpContext.Current.Session["StateId"] != null)
                //{
                //    stateId = Convert.ToInt32(HttpContext.Current.Session["StateId"]);
                //}

                return await _service.GetAllVidhanSabhaAsync(paging, prabhari, StateId);
            });
        }



        [HttpPost]
        [Route("SaveVidhanSabhaRegistration")]
        public async Task<IHttpActionResult> SaveVidhanSabhaRegistration()
        {
           
                int? VidhansabhaId = null;
                //int stateId = 0;

                //if (HttpContext.Current?.Session?["StateId"] != null)
                //{
                //    stateId = Convert.ToInt32(HttpContext.Current.Session["StateId"]);
                //}

                if (HttpContext.Current != null &&
                    HttpContext.Current.Session != null &&
                    HttpContext.Current.Session["VidhanSabhaId"] != null)
                {
                    VidhansabhaId = Convert.ToInt32(HttpContext.Current.Session["VidhanSabhaId"]);
                }

                var request = HttpContext.Current.Request;

                bool.TryParse(request.Form["Prabhari"], out bool isPrabhari);
                var model = new VidhanSabhaRegister
                {
                    Id = int.TryParse(request.Form["Id"], out int id) ? id : 0,

                    VidhanSabhaName = request.Form["VidhanSabhaName"],
                    Prabhari = isPrabhari,


                    Name = request.Form["Name"],
                    Email = request.Form["Email"],
                    PhoneNo = request.Form["PhoneNo"],
                    Education = request.Form["Education"],
                    Address = request.Form["Address"],
                    Profession = request.Form["Profession"],

                    StateId = StateId,

                    DistrictId = int.TryParse(request.Form["DistrictId"], out int districtId)
                                ? districtId
                                : 0,

                    Category = int.TryParse(request.Form["Category"], out int category)
                                ? category
                                : (int?)null,

                    Caste = int.TryParse(request.Form["Caste"], out int caste)
                                ? caste
                                : (int?)null,

                    Status = true
                };

                var file = request.Files["Profile"];

                model.Profile = ImageUploadHelper.SaveImage(file, "~/Uploads/VidhanSabha/");


                int savedId = await _service.SaveVidhanSabhaRegistrationAsync(model);

                if (savedId > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = model.Id > 0 ? "Updated Successfully" : "Added Successfully",
                        id = savedId
                    });
                }

                return BadRequest("Save failed");
            
           
        }


        [HttpGet]
        [Route("getVidhanSabhaById")]
        public async Task<IHttpActionResult> getVidhanSabhaById(int id)
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetVidhanSabhaByIdAsync(id);
            });
        }


        [HttpDelete]
        [Route("DeleteVidhanSabha")]
        public async Task<IHttpActionResult> DeleteVidhanSabha(int id)
        {
            return await ProcessDeleteAsync(async () =>
            {
                return await _service.DeleteVidhanSabhaAsync(id);
            });
        }


        [HttpGet]
        [Route("getDistrictByStateCount")]
        public async Task<IHttpActionResult> getDistrictByStateCount(int stateId)
        {
            return await ProcessRequestAsync(async () =>
            {
                var districts = await _service.GetAllDistrictsDataByStateId(stateId);

                return new
                {
                    data = districts,
                    totalRecords = districts.Count
                };
            });
        }

        [HttpGet]
        [Route("GetDashboardCounts")]
        public async Task<IHttpActionResult> GetDashboardCounts()
        {      
            return await ProcessRequestAsync(async () =>
            {
                var data = await _service.GetDashboardCountsAsync();
                return new { success = true, data = data };
            });
        }

        [HttpGet]
        [Route("getVidhanSabhaByState")]
        public async Task<IHttpActionResult> GetVidhanSabhaByState([FromUri] int DistrictId, [FromUri] Pagination paging)
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetVidhanSabhaByStateIdAsync(DistrictId, paging);
            });
        }

        [HttpGet]
        [Route("GetStateWiseVidhanSabhaChart")]
        public async Task<IHttpActionResult> GetStateWiseVidhanSabhaChart()
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetStateWiseVidhanSabhaChartAsync();

            });
        }

        [HttpGet]
        [Route("GetDistrictWiseVidhanSabhaChart")]
        public async Task<IHttpActionResult> GetDistrictWiseVidhanSabhaChart()
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetDistrictWiseVidhanSabhaChartAsync(StateId);

            });
        }

        [HttpGet]
        [Route("GetDesignationType")]
        public async Task<IHttpActionResult> GetDesignationType()
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetDesignationTypeAsync();

            });

        }

        [HttpGet]
        [Route("GetAllStatePrabhari")]
        public async Task<IHttpActionResult> GetAllStatePrabhari([FromUri] Pagination paging){
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetAllStatePrabhariAsync(paging);

            });
        }

        [HttpPost]
        [Route("SaveStatePrabhari")]
        public async Task<IHttpActionResult> SaveStatePrabhari()
        {
          
                var request = HttpContext.Current.Request;
               

                StatePrabhariModel model = new StatePrabhariModel
                {
                    Id = Convert.ToInt32(request["Id"]),
                    State = Convert.ToInt32(request["State"]),
                    PrabhariName = request["PrabhariName"],
                    Email = request["Email"],
                    PhoneNo = request["PhoneNo"],
                    Category = Convert.ToInt32(request["Category"]),
                    SubCaste = Convert.ToInt32(request["SubCaste"]),
                    Education = request["Education"],
                    Profession = request["Profession"],
                    Address = request["Address"],
                    
                };

                // Image Upload
                var file = request.Files["Profile"];

                model.Profile = ImageUploadHelper.SaveImage(file, "~/Uploads/VidhanSabha/");

                var result = await _service.SaveStatePrabhariAsync(model);

                return Ok(new
                {
                    success = true,
                    message = "Saved successfully",
                    data = result
                });

            
           
        }


        [HttpDelete]
        [Route("DeleteStatePrabhari/{id}")]
        public async Task<IHttpActionResult> DeleteStatePrabhari(int id)
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.DeleteStatePrabhariAsync(id);

            });           
        }

        [HttpGet]
        [Route("GetStatePrabhariById/{id}")]
        public async Task<IHttpActionResult> GetStatePrabhariById(int id)
        {
            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetStatePrabhariByIdAsync(id);

            });
        }

    }
}
