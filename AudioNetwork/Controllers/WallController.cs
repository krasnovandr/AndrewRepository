﻿using System.Web.Mvc;
using AudioNetwork.Models;
using AudioNetwork.Services;
using Microsoft.AspNet.Identity;

namespace AudioNetwork.Controllers
{
    public class WallController : Controller
    {
        private readonly WallService _wallService;

        public WallController(
            WallService wallService)
        {
            _wallService = wallService;
        }

        public JsonResult GetWall(string userId)
        {
            return Json(_wallService.GetWall(userId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWallItem(int wallItemId)
        {
            var userId = User.Identity.GetUserId();
            return Json(_wallService.GetWallItem(userId, wallItemId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddWallItem(WallItemViewModel wallItemView)
        {
            _wallService.AddWallItem(wallItemView);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveWallItem(int wallItemId)
        {
            var userId = User.Identity.GetUserId();
            _wallService.RemoveWallItem(userId, wallItemId);

            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}