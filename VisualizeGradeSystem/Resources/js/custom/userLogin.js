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
function modifypassword() {
    var password1 = jQuery("#new_pass1").val();
    var password2 = jQuery("#new_pass2").val();
    var old_pass = jQuery("#old_pass").val();
    password1 = $.md5(password1).toLocaleUpperCase();
    password2 = $.md5(password2).toLocaleUpperCase();
    old_pass = $.md5(old_pass).toLocaleUpperCase();
    alert("aaa");
    if (password1 != password2)
    {
        alert("aaa");
        jQuery("#error_info").text("两次密码不一致");
        return;
    }
    jQuery.post(
    "/Student/Modify/",
        {
            "old_pass": old_pass,
            "new_pass": password1,
        },
        function (data) {
            if (data == "error") {
                jQuery("#error_info").text("旧密码错误，请重新输入");
            }else {
                location.href = "/Student/Index/";
            }
        }
    );
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
                }
                else if (data == "teacher")
                {
                    location.href = "/Teacher/Index/";
                }
                else if (data == "head") {
                    location.href = "/Head/Index/";
                }
                else {
                    location.href = "/Admin/Index/";
                }
            }
        );
}