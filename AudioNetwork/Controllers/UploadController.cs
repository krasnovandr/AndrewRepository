using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using AudioNetwork.Models;
using AudioNetwork.Services;
using Microsoft.AspNet.Identity;

namespace AudioNetwork.Controllers
{
    public class UploadController : Controller
    {
        private readonly IUploadService _uploadService;
        private readonly IPlaylistService _playlistService;

        public UploadController(
            IUploadService uploadService,
          IPlaylistService playlistService)
        {
            _uploadService = uploadService;
            _playlistService = playlistService;
        }

        public ActionResult Upload()
        {
            return View("Upload");
        }

        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var imageId = Guid.NewGuid().ToString();
                var userId = User.Identity.GetUserId();
                var imagePath = FilePathContainer.ImagePathRelative + imageId + Path.GetExtension(file.FileName);
                var path = Path.Combine(Server.MapPath(FilePathContainer.ForImagePhysicalPath), imageId + Path.GetExtension(file.FileName));
                file.SaveAs(path);
                _uploadService.UploadUserImage(imagePath, userId);
            }

            return Json(null);
        }

        public JsonResult UploadConversationImage(HttpPostedFileBase file, string conversationId, string formData)
        {
            if (file != null && file.ContentLength > 0)
            {
                var imageId = Guid.NewGuid().ToString();
                var imagePath = FilePathContainer.ConversationCoversPathRelative + imageId + Path.GetExtension(file.FileName);
                var path = Path.Combine(Server.MapPath(FilePathContainer.ForConversationCoversPhysicalPath), imageId + Path.GetExtension(file.FileName));
                file.SaveAs(path);

                _uploadService.UploadConversationImage(imagePath, conversationId);
            }

            return Json(null);
        }

        public ActionResult UploadSong(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var absoluteSongPath = Server.MapPath(FilePathContainer.SongPathRelative);
                var absoluteSongCoverPath = Server.MapPath(FilePathContainer.SongAlbumCoverPathPhysical);
                var userId = User.Identity.GetUserId();

                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var songId = Guid.NewGuid().ToString();
                var pathSong = Path.Combine(absoluteSongPath, songId + fileExtension);

                file.SaveAs(pathSong);

                _uploadService.UploadSong(fileExtension, fileName, pathSong, songId, absoluteSongCoverPath, userId);
            }

            return RedirectToAction("GetUser", "Users");
        }

        public JsonResult UploadSongVk(VkUserModel model)
        {
            var userId = User.Identity.GetUserId();
            return Json(_uploadService.GetSongVk(userId, model), JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveSongVk(string songId)
        {
            var userId = User.Identity.GetUserId();

            return Json(new { Success = _playlistService.RemoveSongFromPlaylist(songId, userId + "VK") }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSong(SongViewModel song)
        {
            var userid = User.Identity.GetUserId();
            _uploadService.SaveSongFromVkUpdatePicture(song, userid);

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSongs(List<SongViewModel> songs)
        {
            var userid = User.Identity.GetUserId();
            foreach (var song in songs)
            {
                _uploadService.SaveSongFromVkUpdatePicture(song, userid);

            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}