var i = 0;
function OnClick()
{
	if (i % 2 == 0) {
		$('.collapse').collapse('show');
		i++;
	}
	else {
		$('.collapse').collapse('hide');
		i++;
	}
}