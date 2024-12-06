// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showUserInfo(userId) {
    console.log('Fetching user info for user ID:', userId);
    $.ajax({
        url: '/User/GetUserInfo',
        type: 'GET',
        data: { id: userId },
        success: function (data) {
            console.log('User data loaded successfully');
            $('#userInfoContent').html(data);
            $('#userInfoModal').modal('show');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Failed to load user information:', textStatus, errorThrown);
            alert('Failed to load user information. Please try again.');
        }
    });
}

function showStrayReportDetails(reportId) {
    console.log('Fetching stray report details for report ID:', reportId);
    $.ajax({
        url: '/StrayReport/GetStrayReportDetails',
        type: 'GET',
        data: { id: reportId },
        success: function (data) {
            console.log('Stray report data loaded successfully');
            $('#strayReportContent').html(data);
            $('#strayReportModal').modal('show');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Failed to load stray report details:', textStatus, errorThrown);
            alert('Failed to load stray report details. Please try again.');
        }
    });
}

