angular.module('AudioNetworkApp').factory('wallService', function ($http) {
    return {
        getWall: function (userId) {
            return $http({ method: 'POST', url: 'Wall/GetWall', data: userId });
        },
        getWallItem: function (wallItemId) {
            return $http({ method: 'POST', url: 'Wall/GetWallItem', data: wallItemId });
        },
    
        addWallItem: function (wallItem) {
            return $http({ method: 'POST', url: 'Wall/AddWallItem', data: wallItem });
        },
        removeWallItem: function (wallItem) {
            return $http({ method: 'POST', url: 'Wall/RemoveWallItem', data: wallItem });
        },

        //[HttpGet]
        //public ActionResult GetWall(string userId)
        //{
        //    var wall = _wallRepository.GetWall(userId);
        //    var wallView = new List<WallItemViewModel>();
        //    wallView.AddRange(wall.Select(ModelConverters.ToWallItemViewModel));
        //    return Json(wallView, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult GetWallIem(int wallItemId)
        //{
        //    var userId = User.Identity.GetUserId();
        //    var wallItem = _wallRepository.GetWallIem(userId, wallItemId);
        //    return Json(ModelConverters.ToWallItemViewModel(wallItem), JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult AddWallItem(WallItemViewModel wallItemView)
        //{
        //    var userId = User.Identity.GetUserId();
        //    _wallRepository.AddWallItem(userId, ModelConverters.ToWallItemModel(wallItemView));
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public ActionResult RemoveWallItem(int wallItemId)
        //{
        //    var userId = User.Identity.GetUserId();
        //    _wallRepository.RemoveWallItem(userId, wallItemId);
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}

    };
});