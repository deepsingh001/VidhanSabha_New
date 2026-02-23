using DocumentFormat.OpenXml.Office2010.Excel;
using GlobalVidhanSabha.Helpers;
using GlobalVidhanSabha.Models.AdminMain;
using GlobalVidhanSabha.Models.SamithiMember;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;

using static GlobalVidhanSabha.Models.SamithiMember.VidhanSabhaModel;

[RoutePrefix("api/samithimember")]

public class SamithiMemberController : BaseApiController, IRequiresSessionState
{
    private readonly ISamithiMemberService _service;

    public SamithiMemberController()
    {
        _service = new SamithiMemberService();
    }
    
    private int? GetInt(string value)
    {
        if (int.TryParse(value, out int result))
            return result;

        return null;
    }

    private int StateId
    {
        get{
            return HttpContext.Current?.Session?["StateId"] != null
                ? Convert.ToInt32(HttpContext.Current.Session["StateId"])
                : 0;
        }
    }

    [HttpPost]
    [Route("save")]
    public async Task<IHttpActionResult> Save()
    {
       
            var request = HttpContext.Current.Request;
       
         

            SamithiMemberModel model = new SamithiMemberModel
            {
                Id = GetInt(request.Form["Id"]),
                DesignationType = GetInt(request.Form["DesignationType"]),
                DesignationNameId = GetInt(request.Form["DesignationNameId"]),
                SamithiMember = request.Form["SamithiMember"],
                Email = request.Form["Email"],
                PhoneNo = request.Form["PhoneNo"],
                Category = GetInt(request.Form["Category"]),
                Caste = GetInt(request.Form["Caste"]),
                Education = request.Form["Education"],
                Profession = request.Form["Profession"],
                Address = request.Form["Address"],

                stateId = StateId
            };

            var file = request.Files["Profile"];

            model.ProfilePath = ImageUploadHelper.SaveImage(file, "~/Uploads/VidhanSabha/");

            bool isUpdate = model.Id.HasValue && model.Id > 0;

            return await ProcessCreateOrUpdateAsync(async () =>
            {
                int result = await _service.SaveMemberAsync(model);

                return result > 0 ? result : (int?)null;

            }, isUpdate);
       
    }

    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IHttpActionResult> Delete(int id)
    {
        return await ProcessDeleteAsync(async () =>
        {
            int result = await _service.DeleteMemberAsync(id);
            return result > 0; 
        });
    }

    [HttpGet]
    [Route("getall")]
    public async Task<IHttpActionResult> GetAll([FromUri] Pagination paging)
    {     
        return await ProcessRequestAsync(async () =>
        {
            return await _service.GetAllMembersAsync(StateId, paging);
        });
    }

    [HttpGet]
    [Route("get/{id}")]
    public async Task<IHttpActionResult> GetById(int id)
    {    
        return await ProcessRequestAsync(async () =>
        {
            return await _service.GetMemberByIdAsync(id);
        });
    }

    [HttpGet]
    [Route("dashboardcount")]
    public async Task<IHttpActionResult> DashboardCount()
    {
        return await ProcessRequestAsync(async () =>
            {
                return await _service.GetDashboardCountAsync(StateId);
            });    
    }    
}
