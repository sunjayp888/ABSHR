using HR.Business.Interfaces;
using HR.Business.Models;
using HR.Entity;
using HR.Entity.Dto;
using HR.Extensions;
using HR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace HR.Controllers
{
    [Authorize]
    public class JobTitleDocumentController : BaseController
    {
        public JobTitleDocumentController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: JobTitleDocument
        [HttpGet]
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }
        
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(JobTitleDocument jobTitleDocument)
        {
            try
            {
                List<string> errorList = this.UploadErrorList(jobTitleDocument.Attachment.FileName, jobTitleDocument.Attachment.ContentLength);
                if (errorList.Count == 0)
                {
                    HRBusinessService.CreateJobTitleDocument(UserOrganisationId, jobTitleDocument, User.Identity.Name);
                }
                else
                {
                    return this.JsonNet(errorList);
                }
                return this.JsonNet(
                     ModelState.Values.Where(e => e.Errors.Count > 0)
                         .Select(e => e.Errors.Select(d => d.ErrorMessage).FirstOrDefault())
                         .Distinct());

            }
            catch(Exception ex)
            {
                return this.JsonNet(ex);
            }
                
        }
        
        [HttpPost]
        public ActionResult List(int? id, Paging paging)
        {
            try
            {
                return this.JsonNet(HRBusinessService.RetrieveJobTitleDocuments(UserOrganisationId, id.Value, paging));
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }

        // GET: Personnel/Document/{id}
        [HttpGet]
        public ActionResult Document(Guid id)
        {
            var jobTitleDocument = HRBusinessService.RetrieveJobTitleDocument(UserOrganisationId, id);
            if (jobTitleDocument == null)
            {
                return new HttpNotFoundResult();
            }
            else
            {
                return this.DownloadFile(jobTitleDocument.DocumentFileName, jobTitleDocument.DocumentBytes);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                HRBusinessService.DeleteJobTitleDocument(UserOrganisationId, id);
                return this.JsonNet("");
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }

    }
}