﻿import { showPopup } from '../../shared/popup';
import { hmsFetch, fetchResponses } from "../../shared/hmsFetch.js";

export function showVerifiedAcceptPopup(acceptBtn) {
    let popupMessage =
        `<p>It will appear on your "My Accepted Requests" page and you’ll be able to view more information about it.</p>
            <p>Please use the information in the request to fulfil it as soon as possible (this may involve contacting the recipient) – the ball’s in your court, someone may be depending on you.</p>
            <p>Thank you for helping people in your community to stay safe.</p>`
    var jobId = acceptBtn.parentsUntil(".job").parent().attr("id");
    showPopup({
        header: "Accept this Request for Help?",
        htmlContent: popupMessage,
        messageOnFalse: "We couldn't accept this request at the moment, please try again later",
        actionBtnText: "Accept",
        acceptCallbackAsync: async () => {
            let hasUpdated = await SetRequestToInProgress(jobId);   
                if (hasUpdated == true) {
                    acceptBtn.text("Accepted");
                    acceptBtn.addClass("actioned");
                    acceptBtn.attr("disabled", "true");
                    let acceptLink = acceptBtn.next(".job__info__footer.actioned")
                    acceptLink.show();
                    acceptLink.next(".job__info__footer").hide();
                }
                return hasUpdated;
            }        
    })
}

export function showUnVerifiedAcceptPopup() {
    let popupMessage =
        `<p><a href="/account/profile">Complete your ID verification</a> today so that you can start helping people!</p>
            <p>You're currently registered as an Unverified volunteer. As soon as you've completed your ID Verification, you'll be a Verified volunteer on HelpMyStreet.org and will be able to help people in your local area.</p>`
    showPopup({
        header: "Start Helping",
        htmlContent: popupMessage,
        cssClass: "warning",
        messageOnFalse: "an error occured, please close the popup.",
        actionBtnText: "Get Verified",
        acceptCallbackAsync: () => {
            window.location.href = "/account/profile"
            return true;
        }

    })
}


export async function SetRequestToInProgress(jobId) {
    try {
        let resp = await hmsFetch('/api/requesthelp/accept-request', {
            method: 'post',
            headers: {
                'content-type': 'application/json'
            },
            body: JSON.stringify({ jobId })
        });

        if (resp.fetchRespone == fetchResponses.SUCCESS) {
            return resp.fetchPayload;
        } else {
            return false;
        }
    } catch (err) {
        console.error(err);
        return false;
    }
}

