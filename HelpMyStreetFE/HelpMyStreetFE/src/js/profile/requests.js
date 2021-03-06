import {
    getParameterByName,
    updateQueryStringParam,
    removeQueryStringParam
} from "../shared/querystring-helper";
import {
    showUnVerifiedAcceptPopup,
} from "./requests-popup-helper/open-requests"
import {
    buttonLoad,
    buttonUnload
} from "../shared/btn";
import {
    showPopup
} from "../shared/popup";
import {
    getPopupMessaging
} from "./requests-popup-helper/requests-popup-messaging";
import {
    hmsFetch,
    fetchResponses
} from "../shared/hmsFetch";

export function initialiseRequests(isVerified) {
  const job = getParameterByName("j");

  if (job) {
    var jobEl = $(`#${job}`);
    if (jobEl.length) {
      $("html, body").animate(
        {
          scrollTop: jobEl.offset().top,
        },
        {
          duration: 1000,
          complete: () => {
            if (isVerified) {
              $(`#${job} .job__detail`).slideDown();
              $(`#${job}`).addClass("open highlight");
              loadJobDetails(jobEl);
            } else {
              $(`#${job}`).addClass("highlight");
            }
          },
        }
      );
    }
  }

  $('.job-list').on('mouseover', '.job', function () {
    loadJobDetails($(this));
  });

  $('.job-list').on('click', '.job a.open', function (e) {
    e.preventDefault();
    const job = $(this).closest('.job');
    if (isVerified) {
      updateQueryStringParam('j', $(job).attr('id'));
      job.toggleClass('open');
      job.find('.job__detail').slideToggle();
      loadJobDetails(job);
    } else {
      showUnVerifiedAcceptPopup();
    }
  });

  $('.job-list').on('click', '.job a.close', function (e) {
    e.preventDefault();
    const job = $(this).closest('.job');
    removeQueryStringParam('j', $(job).attr('id'));
    job.toggleClass('open');
    job.find('.job__detail').slideToggle();
  });

  $('.job-list').on('click', '.job__expander h4', function (e) {
    e.preventDefault();
    $(this).toggleClass('open');
    $(this).next().slideToggle();
  });

  $('.job-list').on('click', '.job button.trigger-status-update-popup', function () {
    showStatusUpdatePopup($(this));
  });

  $('.job-list').on('click', '.accept-request-unverified', function () {
    showUnVerifiedAcceptPopup();
  });

  $('.job-list').on('click', '.undo-request', async function (evt) {
    const job = $(this).closest(".job");
    const targetState = $(this).data("target-state");
    const targetUser = $(this).data("target-user") ?? "";

    buttonLoad($(this));
    let response = await setJobStatus(job, targetState, targetUser);
    if (response.fetchResponse == fetchResponses.SUCCESS) {
      $(job).find('.job__status__new').html('');
      $(job).find('.job__info__urgency__dates').toggle();
      $(job).find('button').toggle();
      $(job).find('.next-step').toggle();
    }
    buttonUnload($(this));
  });
}



export function showStatusUpdatePopup(btn) {
  const job = btn.closest(".job");
  const targetState = $(btn).data("target-state");
  const targetUser = $(btn).data("target-user") ?? "self";

  let popupSettings = getPopupMessaging($(job).data("job-status"), targetState, $(job).data("user-acting-as-admin") === "True", $(job).data("referring-group-name"));
  popupSettings.messageOnFalse_Base = popupSettings.messageOnFalse;

  popupSettings.acceptCallbackAsync = async () => {
    let response = await setJobStatus(job, targetState, targetUser);

    if (response.fetchResponse == fetchResponses.SUCCESS) {
      $(job).find('.job__status__new').html(response.fetchPayload);
      $(job).find('.job__info__urgency__dates').toggle();
      $(job).find('button').toggle();
      $(job).find('.next-step').toggle();
      return true;
    } else {
      switch (response.fetchResponse) {
        case fetchResponses.UNAUTHORISED:
        case fetchResponses.BAD_REQUEST:
          popupSettings.messageOnFalse = popupSettings.messageOnFalse_Base + " Another user may have updated the same request; please refresh your browser window.";
          break;
        case fetchResponses.SERVER_ERROR:
        case fetchResponses.SERVER_NOT_FOUND:
        case fetchResponses.TIMEOUT:
        case fetchResponses.BAD_FETCH:
          popupSettings.messageOnFalse = popupSettings.messageOnFalse_Base + " Please try again using the button below.";
      }
      return false;
    }
  };

  showPopup(popupSettings);
}




async function setJobStatus(job, newStatus, targetUser) {
    let jobId = job.attr("id");

    return await hmsFetch('/api/requesthelp/set-job-status?j=' + jobId + '&s=' + newStatus + '&u=' + targetUser);
}


async function loadJobDetails(job, forceRefresh) {
  const jobDetail = $(job).find('.job__detail');

  if (!forceRefresh && jobDetail.data('status') !== undefined) {
    return;
  }

  const jobId = $(job).attr("id");
  const jobSet = $('input[name="JobSet"]').val();
  jobDetail.data('status', 'updating');
  const response = await hmsFetch('/api/requesthelp/get-job-details?j=' + jobId + '&js=' + jobSet);
  if (response.fetchResponse == fetchResponses.SUCCESS) {
    jobDetail.html(await response.fetchPayload);
    jobDetail.data('status', { 'updated': new Date() });
  } else {
    jobDetail.removeData('status');
    return false;
  }
}
