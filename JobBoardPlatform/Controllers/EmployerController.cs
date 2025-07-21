<<<<<<< HEAD
﻿using JobBoardPlatform.Models;
=======
﻿// ===============================
// EmployerController.cs (Updated)
// ===============================

using JobBoardPlatform.Models;
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;

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
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
                    ApplicationCount = 0
                });
            }
            dr.Close();

<<<<<<< HEAD
=======
            // 2️⃣ Application count per job
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
            foreach (var job in jobs)
            {
                string countQuery = "SELECT COUNT(*) FROM Applications WHERE JobId = @JobId";
                SqlCommand countCmd = new SqlCommand(countQuery, con);
                countCmd.Parameters.AddWithValue("@JobId", job.Id);
                job.ApplicationCount = (int)countCmd.ExecuteScalar();
            }

<<<<<<< HEAD
            string statQuery = @"SELECT Status, COUNT(*) AS Count
                                 FROM Applications A
                                 JOIN Jobs J ON A.JobId = J.Id
                                 WHERE J.PostedBy = @PostedBy
                                 GROUP BY Status";
=======
            // 3️⃣ Total/pending/approved/rejected applications
            string statQuery = @"SELECT Status, COUNT(*) AS Count
                             FROM Applications A
                             JOIN Jobs J ON A.JobId = J.Id
                             WHERE J.PostedBy = @PostedBy
                             GROUP BY Status";
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1

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
    {
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

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (U.Name LIKE @Search OR J.Title LIKE @Search)";
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query += " AND A.Status = @StatusFilter";
            }

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@JobId", id);

            if (!string.IsNullOrEmpty(searchTerm))
                cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");
<<<<<<< HEAD
=======

>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
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
    public ActionResult ShortlistedCandidates(string searchTerm = "")
    {
        int empId = Convert.ToInt32(Session["UserId"]);
        List<Application> applications = new List<Application>();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"
<<<<<<< HEAD
                SELECT A.*, U.Name AS CandidateName, J.Title AS JobTitle
                FROM Applications A
                JOIN Users U ON A.UserId = U.Id
                JOIN Jobs J ON A.JobId = J.Id
                WHERE A.Status = 'Approved' AND J.PostedBy = @PostedBy
                  AND (U.Name LIKE @Search OR J.Title LIKE @Search)";
=======
            SELECT A.*, U.Name AS CandidateName, J.Title AS JobTitle
            FROM Applications A
            JOIN Users U ON A.UserId = U.Id
            JOIN Jobs J ON A.JobId = J.Id
            WHERE A.Status = 'Approved' AND J.PostedBy = @PostedBy
              AND (U.Name LIKE @Search OR J.Title LIKE @Search)";
>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1

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
=======







>>>>>>> d2359dde51dcf066c3b29c5bd103a375913dd5d1
}
