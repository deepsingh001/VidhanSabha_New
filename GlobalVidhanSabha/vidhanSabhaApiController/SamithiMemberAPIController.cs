using DocumentFormat.OpenXml.Office2010.Excel;
using GlobalVidhanSabha.Models.AdminMain;
using GlobalVidhanSabha.Models.SamithiMember;
using System;
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

    [HttpPost]
    [Route("save")]
    public async Task<IHttpActionResult> Save()
    {
        try
        {
            var request = HttpContext.Current.Request;

            int? VidhansabhaId = null;

            if (HttpContext.Current != null &&
                HttpContext.Current.Session != null &&
                HttpContext.Current.Session["VidhanSabhaId"] != null)
            {
                VidhansabhaId = Convert.ToInt32(HttpContext.Current.Session["VidhanSabhaId"]);
            }

            SamithiMemberModel model = new SamithiMemberModel
            {
                Id = GetInt(request.Form["Id"]),
                DesignationType = GetInt(request.Form["DesignationType"]),
                SamithiMember = request.Form["SamithiMember"],
                Email = request.Form["Email"],
                PhoneNo = request.Form["PhoneNo"],
                Category = GetInt(request.Form["Category"]),
                Caste = GetInt(request.Form["Caste"]),
                Education = request.Form["Education"],
                Profession = request.Form["Profession"],
                Address = request.Form["Address"],

                VidhanSabhaId = VidhansabhaId
            };

            // FILE UPLOAD
            if (request.Files.Count > 0)
            {
                var file = request.Files["Profile"];

                if (file != null && file.ContentLength > 0)
                {
                    string folder = HttpContext.Current.Server.MapPath("~/Uploads/Profile");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    string fullPath = Path.Combine(folder, fileName);

                    file.SaveAs(fullPath);

                    model.ProfilePath = "/Uploads/Profile/" + fileName;
                }
            }

            bool isUpdate = model.Id.HasValue && model.Id > 0;

            return await ProcessCreateOrUpdateAsync(async () =>
            {
                int result = await _service.SaveMemberAsync(model);

                return result > 0 ? result : (int?)null;

            }, isUpdate);
        }
        catch (Exception ex)
        {
            return InternalServerError(ex);
        }
    }



    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IHttpActionResult> Delete(int id)
    {
        return await ProcessDeleteAsync(async () =>
        {
            int result = await _service.DeleteMemberAsync(id);

            return result > 0; // convert to bool
        });
    }


    [HttpGet]
    [Route("getall")]
    public async Task<IHttpActionResult> GetAll()
    {
        int? vidhanSabhaId = null;

        if (HttpContext.Current != null &&
            HttpContext.Current.Session != null &&
            HttpContext.Current.Session["VidhanSabhaId"] != null)
        {
            vidhanSabhaId = Convert.ToInt32(HttpContext.Current.Session["VidhanSabhaId"]);
        }

        return await ProcessRequestAsync(async () =>
        {
            return await _service.GetAllMembersAsync(vidhanSabhaId);
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
        
            int? vidhanSabhaId = null;

            if (HttpContext.Current != null &&
                HttpContext.Current.Session != null &&
                HttpContext.Current.Session["VidhanSabhaId"] != null)
            {
                vidhanSabhaId = Convert.ToInt32(HttpContext.Current.Session["VidhanSabhaId"]);
            }

            return await ProcessRequestAsync(async () =>
            {
                return await _service.GetDashboardCountAsync(vidhanSabhaId);
            });
        
       
    }

}
