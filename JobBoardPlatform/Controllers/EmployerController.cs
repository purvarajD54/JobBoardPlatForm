<<<<<<< HEAD
﻿using JobBoardPlatform.Models;
using OfficeOpenXml;
=======
<<<<<<< HEAD
﻿using JobBoardPlatform.Models;
=======
﻿// ===============================
// EmployerController.cs (Updated)
// ===============================

using JobBoardPlatform.Models;
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml.Style;
using System.Drawing;

public class EmployerController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["JobBoardDB"].ConnectionString;

    public ActionResult Dashboard(string searchTerm = "")
    {
        int empId = Convert.ToInt32(Session["UserId"]);
        List<Job> jobs = new List<Job>();
        int totalApplications = 0, approvedJobs = 0, pendingApps = 0, approvedApps = 0, rejectedApps = 0;

        using (SqlConnection con = new SqlConnection(conStr))
        {
<<<<<<< HEAD

            con.Open();

            string jobQuery = @"SELECT * FROM Jobs 
                                WHERE PostedBy=@PostedBy
                                AND (Title LIKE @Search OR Category LIKE @Search)";
=======
            con.Open();

<<<<<<< HEAD
            string jobQuery = @"SELECT * FROM Jobs 
                                WHERE PostedBy=@PostedBy
                                AND (Title LIKE @Search OR Category LIKE @Search)";
=======
            // 1️⃣ Load jobs for employer with optional search filter
            string jobQuery = @"SELECT * FROM Jobs 
                            WHERE PostedBy=@PostedBy
                            AND (Title LIKE @Search OR Category LIKE @Search)";
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35
            SqlCommand jobCmd = new SqlCommand(jobQuery, con);
            jobCmd.Parameters.AddWithValue("@PostedBy", empId);
            jobCmd.Parameters.AddWithValue("@Search", $"%{searchTerm}%");

            SqlDataReader dr = jobCmd.ExecuteReader();
            while (dr.Read())
            {
                var jobId = (int)dr["Id"];
                bool isApproved = (bool)dr["IsApproved"];
                if (isApproved) approvedJobs++;

                jobs.Add(new Job
                {
                    Id = jobId,
                    Title = dr["Title"].ToString(),
                    Description = dr["Description"].ToString(),
                    Category = dr["Category"].ToString(),
                    Location = dr["Location"].ToString(),
                    PostedDate = (DateTime)dr["PostedDate"],
                    IsApproved = isApproved,
<<<<<<< HEAD
                    ImagePath = dr["ImagePath"]?.ToString(),
=======
<<<<<<< HEAD
                    ImagePath = dr["ImagePath"]?.ToString(),
=======
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35
                    ApplicationCount = 0
                });
            }
            dr.Close();

<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
            // 2️⃣ Application count per job
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35
            foreach (var job in jobs)
            {
                string countQuery = "SELECT COUNT(*) FROM Applications WHERE JobId = @JobId";
                SqlCommand countCmd = new SqlCommand(countQuery, con);
                countCmd.Parameters.AddWithValue("@JobId", job.Id);
                job.ApplicationCount = (int)countCmd.ExecuteScalar();
            }

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35
            string statQuery = @"SELECT Status, COUNT(*) AS Count
                                 FROM Applications A
                                 JOIN Jobs J ON A.JobId = J.Id
                                 WHERE J.PostedBy = @PostedBy
                                 GROUP BY Status";
<<<<<<< HEAD
=======
=======
            // 3️⃣ Total/pending/approved/rejected applications
            string statQuery = @"SELECT Status, COUNT(*) AS Count
                             FROM Applications A
                             JOIN Jobs J ON A.JobId = J.Id
                             WHERE J.PostedBy = @PostedBy
                             GROUP BY Status";
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35

            SqlCommand statCmd = new SqlCommand(statQuery, con);
            statCmd.Parameters.AddWithValue("@PostedBy", empId);
            SqlDataReader statDr = statCmd.ExecuteReader();

            while (statDr.Read())
            {
                string status = statDr["Status"].ToString();
                int count = (int)statDr["Count"];

                totalApplications += count;
                if (status == "Applied") pendingApps = count;
                else if (status == "Approved") approvedApps = count;
                else if (status == "Rejected") rejectedApps = count;
            }
        }

        ViewBag.TotalJobs = jobs.Count;
        ViewBag.ApprovedJobs = approvedJobs;
        ViewBag.TotalApplications = totalApplications;
        ViewBag.PendingApplications = pendingApps;
        ViewBag.ApprovedApplications = approvedApps;
        ViewBag.RejectedApplications = rejectedApps;
        ViewBag.SearchTerm = searchTerm;

        return View(jobs);
    }








    public ActionResult CreateJob() => View();

    [HttpPost]
    public ActionResult PostJob(Job job, HttpPostedFileBase ImageFile)
    {
        if (ImageFile != null && ImageFile.ContentLength > 0)
        {
            var fileName = Path.GetFileName(ImageFile.FileName);
            var uploadDir = Server.MapPath("~/Uploads");

            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            var filePath = Path.Combine(uploadDir, fileName);
            ImageFile.SaveAs(filePath);

            job.ImagePath = "/Uploads/" + fileName;
        }

        job.PostedBy = Convert.ToInt32(Session["UserId"]);
        job.PostedDate = DateTime.Now;
        job.IsApproved = false;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"INSERT INTO Jobs 
                            (Title, Description, Category, Location, PostedBy, PostedDate, IsApproved, ImagePath)
                             VALUES 
                            (@Title, @Description, @Category, @Location, @PostedBy, @PostedDate, @IsApproved, @ImagePath)";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Title", job.Title);
            cmd.Parameters.AddWithValue("@Description", job.Description);
            cmd.Parameters.AddWithValue("@Category", job.Category);
            cmd.Parameters.AddWithValue("@Location", job.Location);
            cmd.Parameters.AddWithValue("@PostedBy", job.PostedBy);
            cmd.Parameters.AddWithValue("@PostedDate", job.PostedDate);
            cmd.Parameters.AddWithValue("@IsApproved", job.IsApproved);
            cmd.Parameters.AddWithValue("@ImagePath", job.ImagePath ?? (object)DBNull.Value);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        return RedirectToAction("Dashboard");
    }

<<<<<<< HEAD
    public ActionResult EditJob(int id)
=======
<<<<<<< HEAD
=======
    // GET: Employer/EditJob/5
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
    public ActionResult EditJob(int id)
    {
        Job job = new Job();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "SELECT * FROM Jobs WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                job.Id = (int)dr["Id"];
                job.Title = dr["Title"].ToString();
                job.Description = dr["Description"].ToString();
                job.Category = dr["Category"].ToString();
                job.Location = dr["Location"].ToString();
<<<<<<< HEAD
                job.ImagePath = dr["ImagePath"]?.ToString();
=======
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
            }
        }

        return View(job);
    }

<<<<<<< HEAD

    [HttpPost]
    public ActionResult Edit(Job job, HttpPostedFileBase ImageFile)
    {
        if (ModelState.IsValid)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["JobBoardDB"].ConnectionString))
            {
                con.Open();

                string imagePath = job.ImagePath; // default = old image path

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string imageFolder = "~/Uploads/";
                    string physicalPath = Server.MapPath(imageFolder + fileName);
                    ImageFile.SaveAs(physicalPath);
                    imagePath = imageFolder + fileName;
                }

                string query = "UPDATE Jobs SET Title=@Title, Description=@Description, Category=@Category, Location=@Location, ImagePath=@ImagePath WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Title", job.Title);
                cmd.Parameters.AddWithValue("@Description", job.Description);
                cmd.Parameters.AddWithValue("@Category", job.Category);
                cmd.Parameters.AddWithValue("@Location", job.Location);
                cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                cmd.Parameters.AddWithValue("@Id", job.Id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Dashboard", "Employer");
        }

        return View(job);
    }


    public ActionResult JobDetails(int id)
    {
        Job job = null;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "SELECT * FROM Jobs WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                job = new Job
                {
                    Id = (int)dr["Id"],
                    Title = dr["Title"].ToString(),
                    Description = dr["Description"].ToString(),
                    Category = dr["Category"].ToString(),
                    Location = dr["Location"].ToString(),
                    PostedDate = (DateTime)dr["PostedDate"],
                    IsApproved = (bool)dr["IsApproved"],
                    ImagePath = dr["ImagePath"]?.ToString()
                };
            }
        }

        if (job == null)
            return HttpNotFound();

        return View(job);
    }

    public ActionResult DeleteJob(int id)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "DELETE FROM Jobs WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
=======
    // POST: Employer/EditJob/5
    [HttpPost]
    public ActionResult EditJob(Job job)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"UPDATE Jobs 
                         SET Title = @Title, Description = @Description, Category = @Category, Location = @Location 
                         WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Title", job.Title);
            cmd.Parameters.AddWithValue("@Description", job.Description);
            cmd.Parameters.AddWithValue("@Category", job.Category);
            cmd.Parameters.AddWithValue("@Location", job.Location);
            cmd.Parameters.AddWithValue("@Id", job.Id);

>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
            con.Open();
            cmd.ExecuteNonQuery();
        }

<<<<<<< HEAD
        TempData["SuccessMessage"] = "Job deleted successfully!";
        return RedirectToAction("Dashboard");
    }

=======
        return RedirectToAction("Dashboard");
    }


>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
    public ActionResult ViewApplications(int id, string searchTerm, string statusFilter)
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35
    {
        Job job = new Job();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "SELECT * FROM Jobs WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                job.Id = (int)dr["Id"];
                job.Title = dr["Title"].ToString();
                job.Description = dr["Description"].ToString();
                job.Category = dr["Category"].ToString();
                job.Location = dr["Location"].ToString();
                job.ImagePath = dr["ImagePath"]?.ToString();
            }
        }

        return View(job);
    }


    [HttpPost]
    public ActionResult Edit(Job job, HttpPostedFileBase ImageFile)
    {
        if (ModelState.IsValid)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["JobBoardDB"].ConnectionString))
            {
                con.Open();

                string imagePath = job.ImagePath; // default = old image path

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string imageFolder = "~/Uploads/";
                    string physicalPath = Server.MapPath(imageFolder + fileName);
                    ImageFile.SaveAs(physicalPath);
                    imagePath = imageFolder + fileName;
                }

                string query = "UPDATE Jobs SET Title=@Title, Description=@Description, Category=@Category, Location=@Location, ImagePath=@ImagePath WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Title", job.Title);
                cmd.Parameters.AddWithValue("@Description", job.Description);
                cmd.Parameters.AddWithValue("@Category", job.Category);
                cmd.Parameters.AddWithValue("@Location", job.Location);
                cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                cmd.Parameters.AddWithValue("@Id", job.Id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Dashboard", "Employer");
        }

        return View(job);
    }


    public ActionResult JobDetails(int id)
    {
        Job job = null;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "SELECT * FROM Jobs WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                job = new Job
                {
                    Id = (int)dr["Id"],
                    Title = dr["Title"].ToString(),
                    Description = dr["Description"].ToString(),
                    Category = dr["Category"].ToString(),
                    Location = dr["Location"].ToString(),
                    PostedDate = (DateTime)dr["PostedDate"],
                    IsApproved = (bool)dr["IsApproved"],
                    ImagePath = dr["ImagePath"]?.ToString()
                };
            }
        }

        if (job == null)
            return HttpNotFound();

        return View(job);
    }

    public ActionResult DeleteJob(int id)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "DELETE FROM Jobs WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["SuccessMessage"] = "Job deleted successfully!";
        return RedirectToAction("Dashboard");
    }

    public ActionResult ViewApplications(int id, string searchTerm, string statusFilter)
    {
        ViewBag.JobId = id;

        List<Application> applications = new List<Application>();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"SELECT A.*, U.Name AS CandidateName, J.Title AS JobTitle
                             FROM Applications A
                             JOIN Users U ON A.UserId = U.Id
                             JOIN Jobs J ON A.JobId = J.Id
                             WHERE A.JobId = @JobId";

            if (!string.IsNullOrEmpty(searchTerm))
                query += " AND (U.Name LIKE @Search OR J.Title LIKE @Search)";
            if (!string.IsNullOrEmpty(statusFilter))
                query += " AND A.Status = @StatusFilter";
<<<<<<< HEAD
=======

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (U.Name LIKE @Search OR J.Title LIKE @Search)";
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query += " AND A.Status = @StatusFilter";
            }
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@JobId", id);

            if (!string.IsNullOrEmpty(searchTerm))
                cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======

>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35
            if (!string.IsNullOrEmpty(statusFilter))
                cmd.Parameters.AddWithValue("@StatusFilter", statusFilter);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                applications.Add(new Application
                {
                    Id = (int)dr["Id"],
                    JobId = (int)dr["JobId"],
                    UserId = (int)dr["UserId"],
                    ResumePath = dr["ResumePath"].ToString(),
                    AppliedDate = (DateTime)dr["AppliedDate"],
                    Status = dr["Status"].ToString(),
                    CandidateName = dr["CandidateName"].ToString(),
                    JobTitle = dr["JobTitle"].ToString()
                });
            }
        }

        return View(applications);
    }

<<<<<<< HEAD
    public ActionResult ApproveApplication(int id, int jobId)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "UPDATE Applications SET Status = 'Approved' WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("ViewApplications", new { id = jobId });
    }

    public ActionResult RejectApplication(int id, int jobId)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "UPDATE Applications SET Status = 'Rejected' WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("ViewApplications", new { id = jobId });
    }

=======
<<<<<<< HEAD
    public ActionResult ApproveApplication(int id, int jobId)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "UPDATE Applications SET Status = 'Approved' WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("ViewApplications", new { id = jobId });
    }

    public ActionResult RejectApplication(int id, int jobId)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "UPDATE Applications SET Status = 'Rejected' WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("ViewApplications", new { id = jobId });
    }

=======


    public ActionResult ApproveApplication(int id, int jobId)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "UPDATE Applications SET Status = 'Approved' WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("ViewApplications", new { id = jobId });

    }

    public ActionResult RejectApplication(int id, int jobId)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "UPDATE Applications SET Status = 'Rejected' WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("ViewApplications", new { id = jobId });

    }


    public ActionResult JobDetails(int id)
    {
        Job job = null;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "SELECT * FROM Jobs WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                job = new Job
                {
                    Id = (int)dr["Id"],
                    Title = dr["Title"].ToString(),
                    Description = dr["Description"].ToString(),
                    Category = dr["Category"].ToString(),
                    Location = dr["Location"].ToString(),
                    PostedDate = (DateTime)dr["PostedDate"],
                    IsApproved = (bool)dr["IsApproved"]
                };
            }
        }

        if (job == null)
            return HttpNotFound();

        return View(job);
    }

    public ActionResult DeleteJob(int id)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "DELETE FROM Jobs WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["SuccessMessage"] = "Job deleted successfully!";
        return RedirectToAction("Dashboard");
    }

>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35
    public ActionResult ShortlistedCandidates(string searchTerm = "")
    {
        int empId = Convert.ToInt32(Session["UserId"]);
        List<Application> applications = new List<Application>();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35
                SELECT A.*, U.Name AS CandidateName, J.Title AS JobTitle
                FROM Applications A
                JOIN Users U ON A.UserId = U.Id
                JOIN Jobs J ON A.JobId = J.Id
                WHERE A.Status = 'Approved' AND J.PostedBy = @PostedBy
                  AND (U.Name LIKE @Search OR J.Title LIKE @Search)";
<<<<<<< HEAD
=======
=======
            SELECT A.*, U.Name AS CandidateName, J.Title AS JobTitle
            FROM Applications A
            JOIN Users U ON A.UserId = U.Id
            JOIN Jobs J ON A.JobId = J.Id
            WHERE A.Status = 'Approved' AND J.PostedBy = @PostedBy
              AND (U.Name LIKE @Search OR J.Title LIKE @Search)";
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@PostedBy", empId);
            cmd.Parameters.AddWithValue("@Search", $"%{searchTerm}%");

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                applications.Add(new Application
                {
                    Id = (int)dr["Id"],
                    JobId = (int)dr["JobId"],
                    UserId = (int)dr["UserId"],
                    ResumePath = dr["ResumePath"].ToString(),
                    AppliedDate = (DateTime)dr["AppliedDate"],
                    Status = dr["Status"].ToString(),
                    CandidateName = dr["CandidateName"].ToString(),
                    JobTitle = dr["JobTitle"].ToString()
                });
            }
        }

        ViewBag.SearchTerm = searchTerm;
        return View(applications);
    }
<<<<<<< HEAD
    [HttpGet]
    public ActionResult DownloadJobApplicationsExcel(int jobId)
    {
        List<ApplicationViewModel> applications = new List<ApplicationViewModel>();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"SELECT A.Id, U.Name AS CandidateName, J.Title AS JobTitle, 
                        A.AppliedDate, A.Status
                        FROM Applications A
                        JOIN Users U ON A.UserId = U.Id
                        JOIN Jobs J ON A.JobId = J.Id
                        WHERE A.JobId = @JobId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@JobId", jobId);
            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                applications.Add(new ApplicationViewModel
                {
                    CandidateName = dr["CandidateName"].ToString(),
                    JobTitle = dr["JobTitle"].ToString(),
                    AppliedDate = Convert.ToDateTime(dr["AppliedDate"]),
                    Status = dr["Status"].ToString()
                });
            }
        }

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // EPPlus 5–7

        using (ExcelPackage package = new ExcelPackage())
        {
            ExcelWorksheet ws = package.Workbook.Worksheets.Add("Applications");

            // Header
            ws.Cells[1, 1].Value = "Candidate";
            ws.Cells[1, 2].Value = "Job Title";
            ws.Cells[1, 3].Value = "Applied Date";
            ws.Cells[1, 4].Value = "Status";

            using (var headerRange = ws.Cells[1, 1, 1, 4])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            // Data
            for (int i = 0; i < applications.Count; i++)
            {
                var app = applications[i];
                ws.Cells[i + 2, 1].Value = app.CandidateName;
                ws.Cells[i + 2, 2].Value = app.JobTitle;
                ws.Cells[i + 2, 3].Value = app.AppliedDate.ToString("dd-MM-yyyy");
                ws.Cells[i + 2, 4].Value = app.Status;
            }

            var excelBytes = package.GetAsByteArray();
            string filename = $"Applications_Job_{jobId}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
    }

    public ActionResult DownloadShortlistedApplicationsExcel()
    {
        List<ApplicationViewModel> shortlistedApplications = new List<ApplicationViewModel>();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"SELECT A.Id, A.AppliedDate, A.Status, A.ResumePath,
                         U.Name AS CandidateName, J.Title AS JobTitle
                         FROM Applications A
                         INNER JOIN Users U ON A.UserId = U.Id
                         INNER JOIN Jobs J ON A.JobId = J.Id
                         WHERE A.Status = 'Approved'";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                shortlistedApplications.Add(new ApplicationViewModel
                {
                    CandidateName = dr["CandidateName"].ToString(),
                    JobTitle = dr["JobTitle"].ToString(),
                    AppliedDate = Convert.ToDateTime(dr["AppliedDate"]),
                    Status = dr["Status"].ToString()
                });
            }
        }

        // EPPlus License Context (if not already set)
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (ExcelPackage package = new ExcelPackage())
        {
            ExcelWorksheet ws = package.Workbook.Worksheets.Add("Shortlisted");

            // Header
            ws.Cells[1, 1].Value = "Candidate";
            ws.Cells[1, 2].Value = "Job Title";
            ws.Cells[1, 3].Value = "Applied Date";
            ws.Cells[1, 4].Value = "Status";

            using (var range = ws.Cells[1, 1, 1, 4])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            }

            // Data rows
            for (int i = 0; i < shortlistedApplications.Count; i++)
            {
                var app = shortlistedApplications[i];
                ws.Cells[i + 2, 1].Value = app.CandidateName;
                ws.Cells[i + 2, 2].Value = app.JobTitle;
                ws.Cells[i + 2, 3].Value = app.AppliedDate.ToString("dd-MM-yyyy");
                ws.Cells[i + 2, 4].Value = app.Status;
            }

            var fileBytes = package.GetAsByteArray();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Shortlisted_Candidates.xlsx");
        }
    }

    public ActionResult DownloadPostedJobsExcel()
    {
        int employerId = Convert.ToInt32(Session["UserId"]); // ✅ FIXED HERE
        List<Job> jobs = new List<Job>();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"
        SELECT 
            J.Title,
            J.Category,
            J.Location,
            J.PostedDate,
            CASE 
                WHEN J.IsApproved = 1 THEN 'Approved' 
                ELSE 'Pending' 
            END AS Status,
            (
                SELECT COUNT(*) 
                FROM Applications A 
                WHERE A.JobId = J.Id
            ) AS Applications
        FROM Jobs J
        WHERE J.PostedBy = @EmployerId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@EmployerId", employerId);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                jobs.Add(new Job
                {
                    Title = dr["Title"].ToString(),
                    Category = dr["Category"].ToString(),
                    Location = dr["Location"].ToString(),
                    PostedDate = Convert.ToDateTime(dr["PostedDate"]),
                    Status = dr["Status"].ToString(),
                    ApplicationCount = Convert.ToInt32(dr["Applications"])
                });
            }
        }

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (ExcelPackage package = new ExcelPackage())
        {
            var ws = package.Workbook.Worksheets.Add("Posted Jobs");

            ws.Cells[1, 1].Value = "Title";
            ws.Cells[1, 2].Value = "Category";
            ws.Cells[1, 3].Value = "Location";
            ws.Cells[1, 4].Value = "Posted Date";
            ws.Cells[1, 5].Value = "Status";
            ws.Cells[1, 6].Value = "Applications";

            using (var range = ws.Cells[1, 1, 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            for (int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];
                ws.Cells[i + 2, 1].Value = job.Title;
                ws.Cells[i + 2, 2].Value = job.Category;
                ws.Cells[i + 2, 3].Value = job.Location;
                ws.Cells[i + 2, 4].Value = job.PostedDate.ToString("dd-MM-yyyy");
                ws.Cells[i + 2, 5].Value = job.Status;
                ws.Cells[i + 2, 6].Value = job.ApplicationCount;
            }

            ws.Cells.AutoFitColumns();

            var fileBytes = package.GetAsByteArray();
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Posted_Jobs.xlsx");
        }
    }


=======
<<<<<<< HEAD
=======







>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
>>>>>>> f5518f85fefe67dfd304f3ee1ca70b1cf1e24a35
}
