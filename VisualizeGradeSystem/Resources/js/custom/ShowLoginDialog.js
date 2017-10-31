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