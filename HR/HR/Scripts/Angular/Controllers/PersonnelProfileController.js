(function () {
    'use strict';

    angular
        .module('HR')
        .controller('PersonnelProfileController', PersonnelProfileController);

    PersonnelProfileController.$inject = ['$window', 'PersonnelProfileService', 'Paging', 'OrderService', 'OrderBy', 'Order', '$q'];

    function PersonnelProfileController($window, PersonnelProfileService, Paging, OrderService, OrderBy, Order, $q) {
        /* jshint validthis:true */
        var vm = this;
        vm.personnelId;
        vm.initialise = initialise;
        vm.uploadPhoto = uploadPhoto;
        vm.deletePhoto = deletePhoto;

        var cropImage


        //Cropper
        var cropImage = $('#UploadProfilePicture');
        var options = {
            aspectRatio: 1 / 1,
            responsive : true,
            crop: function (e) {
            }
        };

        $('#inputImage').change(function () {
            var files = this.files;
            var file;

            if (!cropImage.data('cropper')) {
                return;
            }

            if (files && files.length) {
                file = files[0];
                if (/^image\/\w+$/.test(file.type)) {
                    var blobURL = URL.createObjectURL(file);
                    cropImage.one('built.cropper', function () {
                        URL.revokeObjectURL(blobURL);
                    }).cropper('reset').cropper('replace', blobURL);
                } else {
                    window.alert('Please choose an image file.');
                }
            }
        });
        //Cropper

        function initialise(personnelId) {
            vm.personnelId = personnelId;
            cropImage.on({
            }).cropper(options);
        }

        function uploadPhoto() {
            getCroppedImage().then(function (blobImage) {
                return PersonnelProfileService.UploadPhoto(vm.personnelId, blobImage)
                    .then(function (response) {
                        $('#ProfilePicture').attr('src', URL.createObjectURL(blobImage));
                        $("#ProfilePictureModal").modal("hide");
                });
            });

        }

        function getCroppedImage() {
            var canvas = cropImage.cropper("getCroppedCanvas", undefined);
            var deferred = $q.defer();
            if (typeof canvas.toBlob !== "undefined") {
                canvas.toBlob(function (blob) {
                    deferred.resolve(blob);
                });
            }
            else if (typeof canvas.msToBlob !== "undefined") {
                deferred.resolve(canvas.msToBlob());
            }
            return deferred.promise;
        }


        function deletePhoto() {
            return PersonnelProfileService.DeletePhoto(vm.personnelId)
                    .then(function (response) {
                        document.getElementById('ProfilePicture').setAttribute('src', location.protocol + '//' + location.host + '/Content/images/user.png');
                        $(".cropper-canvas img").attr('src', location.protocol + '//' + location.host + '/Personnel/Photo/' + vm.personnelId);
                        $(".cropper-view-box img").attr('src', location.protocol + '//' + location.host + '/Personnel/Photo/' + vm.personnelId);
                        $("#UploadProfilePicture").attr('src', '/Personnel/Photo/' + vm.personnelId);
                        $("#ProfilePictureModal").modal("hide");

            });
        }
    }
})();
