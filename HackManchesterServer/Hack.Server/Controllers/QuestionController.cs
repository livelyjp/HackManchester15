﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Hack.Domain.Entities;
using Hack.Domain.Interfaces;
using Hack.EF;

namespace Hack.Server.Controllers
{
    public class QuestionController : BaseController

    {
        public QuestionController(HackDbContext hackDbContext, IApplicationContext applicationContext)
            : base(hackDbContext, applicationContext)
        {
        }

        public ActionResult Details(DetailsModel model)
        {
            var question =
                HackDbContext.Questions.Where(x => x.Id == model.Id)
                .Include(x => x.Offers)
                .Include(x => x.User)
                .Include(x => x.Offers.Select(y => y.SubmittedByUser))
                .First();
            return View(new QuestionDetailViewModel(question));
        }
    }

    public class DetailsModel
    {
        public long Id { get; set; }
        public string Token { get; set; }
    }

    public class QuestionDetailViewModel
    {
        public QuestionDetailViewModel(Question question)
        {
            Id = question.Id;
            Title = question.Title;
            Description = question.Description;
            DateTimeString = DateHelper.DateTimeGenerator(question.QuestionUploadedDateTime);
            UserNameString = "By " + question.User.FullName();
            Comments = question.Offers.Select(x => new CommentItem(x)).ToList();
        }

        public long UserId { get; set; }

        public string ProfileUrl { get; set; }

        public string UserNameString { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public List<CommentItem> Comments { get; set; }
        public string DateTimeString { get; set; }
        public string UserName { get; set; }
        public long Id { get; set; }
    }


    public class CommentItem
    {
        public CommentItem(Offer offer)
        {
            Id = offer.Id;
            Content = offer.Text;
            UserId = offer.SubmittedByUserId;
            VideoCallAvailable = offer.VideoCallAvailable;
            Timestamp = DateHelper.DateTimeGenerator(offer.OfferDateTime);
            UserNameString = offer.SubmittedByUser.FullName();
            ProfileUrl = offer.SubmittedByUser.ProfileImageUrl;
        }

        public long UserId { get; set; }

        public string ProfileUrl { get; set; }

        public bool VideoCallAvailable { get; set; }
        public long Id { get; set; }
        public string Content { get; set; }
        public string Timestamp { get; set; }
        public string UserNameString { get; set; }
    }

    public static class DateHelper
    {
        public static string DateTimeGenerator(DateTime dateTime)
        {
            var elapsedTimeSpan = DateTime.Now - dateTime;
            if (elapsedTimeSpan.Hours >= 1)
            {
                return elapsedTimeSpan.Hours + " Hours Ago";
            }
            return elapsedTimeSpan.Minutes + " Mins Ago";
        }
    }
}