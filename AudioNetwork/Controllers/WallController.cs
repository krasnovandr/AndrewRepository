﻿using System.Web.Mvc;
using AudioNetwork.Models;
using AudioNetwork.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace AudioNetwork.Controllers
{
    public class WallController : Controller
    {
        private readonly IWallService _wallService;

        public WallController(
            IWallService wallService)
        {
            _wallService = wallService;
        }

        public ActionResult ViewNews()
        {
            return View("News");
        }

        public JsonResult GetNews()
        {
            var userId = User.Identity.GetUserId();

            return Json(_wallService.GetUserNews(userId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFriendUpdates()
        {
            var userId = User.Identity.GetUserId();

            return Json(_wallService.GetFriendUpdates(userId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetWall(string userId)
        {
            return Json(_wallService.GetWall(userId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetWallItem(int wallItemId)
        {
            var userId = User.Identity.GetUserId();
            return Json(_wallService.GetWallItem(userId, wallItemId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddWallItem(WallItemViewModel wallItemView)
        {
            _wallService.AddWallItem(wallItemView);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveWallItem(int wallItemId)
        {
            var userId = User.Identity.GetUserId();
            _wallService.RemoveWallItem(userId, wallItemId);

            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}