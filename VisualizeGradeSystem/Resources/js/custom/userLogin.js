function showLoginForm() {
    $('#loginModal .registerBox').fadeOut('fast', function () {
        $('.loginBox').fadeIn('fast');
        $('.register-footer').fadeOut('fast', function () {
            $('.login-footer').fadeIn('fast');
        });

        $('.modal-title').html('Login with');
    });
    $("#error_info").text("");
}
function openLoginModal() {
    showLoginForm();
    setTimeout(function () {
        $('#loginModal').modal('show');
    }, 230);

}

function refreshCode() {
    var codeImg = jQuery("#codeImg");
    var time = new Date().getSeconds();
    var url = "/VerifyCode?time=" + time;
    codeImg.attr("src", url);
}

function login() {

    var password = jQuery("#user_pass").val();
    strmd5 = $.md5(password).toLocaleUpperCase();
    if (jQuery("#user_id").val().length <= 0) {
        jQuery("#error_info").text("请输入账号");
        return;
    }
    if (jQuery("#user_pass").val().length <= 0) {
        jQuery("#error_info").text("请输入密码");
        return;
    }
    if (jQuery("#ValidateCode").val().length <= 0) {
        jQuery("#error_info").text("请输入验证码");
        return;
    }

    jQuery.post(
        "/Login/CheckUser/",
            {
                "account": jQuery("#user_id").val(),
                "password": strmd5,
                "verifycode": jQuery("#ValidateCode").val()
            },
            function (data) {
                if (data == "error") {
                    jQuery("#error_info").text("账号或密码错误");
                    refreshCode();
                }
                else if (data == "ErrorCode") {
                    jQuery("#error_info").text("验证码错误");
                    refreshCode();
                }
                else if (data == "stu") {
                    location.href = "/Student/Index/";
                } else {
                    location.href = "/Teacher/Index/";
                }
            }
        );
}