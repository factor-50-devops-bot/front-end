﻿
import "../sass/main.scss";
import firebase from "./firebase/index";
import account from "./account";
import notification from "./account/notification";
import "./shared/info-popup";
import "./shared/site-header";
import { intialiseCookieConsent } from "./shared/cookie-helper"
import { intialiseForgottonForm } from "./home/forgotton-password"
import { buttonLoad, buttonUnload } from "./shared/btn";

$(function () {
    $('.no-fouc').removeClass('no-fouc');
    if (typeof configuration !== 'undefined') {
        firebase.init(JSON.parse(configuration.firebase));
    }
    window.account = account;

    intialiseCookieConsent();
    intialiseForgottonForm(firebase, account);

  $("#postcode_button").click(function(evt) {
    const postCode = $("#postcode").val();

    if (postCode) {
      $(this).width($(this).width());
      $(this).height($(this).height());

      $("#postcode_notcovered").hide();
      $("#postcode_covered").hide();
      $("#postcode_error").hide();
      $("#postcode_invalid").hide();
      //$("#request_help").hide();        
      $(".postcode__info").hide();
      $('#postcode_button').removeClass('postcode_button_clicked')

        buttonLoad($(this));

        fetch(`/api/postcode/checkCoverage/${postCode}`)
        .then(resp => resp.json())
            .then(data => {
                $('#postcode_button').addClass('postcode_button_clicked')                
                var postCodeValid = (data.postCodeResponse.isSuccessful && data.postCodeResponse.hasContent);                
                if (postCodeValid == false) {
                    $(".postcode__info, #postcode_invalid").show();
                } else {
                    if (data.volunteerCount == 0 && data.championCount == 0)
                        $(".postcode__info, #postcode_notcovered").show();
                    else if (data.volunteerCount > 0 || data.championCount > 0) {
                        $(".postcode__info, #postcode_covered").show();
                        //if (data.volunteerCount > 0 && data.championCount > 0) { // phase.1.1
                        //    $(".postcode__info, #request_help").show();                           
                        //}
                    }                        
                }              
        })
        .catch(err => {
          $("#postcode_error").show();
        })
        .finally(() => {
           $(this).width(null);
            $(this).height(null);
            buttonUnload($(this));
        });
    }
  });

    $("#login-submit").click(async () => {        
        buttonLoad($(this));
    try {
        $("#login-submit")[0].disabled = true;
        const email = $("#email").val();
        const password = $("#password").val();
        const response = await account.login.login(email, password);
        if (!response.success) {
        $("#login-fail-message").text(response.message);
        $("#login-submit")[0].disabled = false;
      }
    } finally {
        buttonUnload($(this));
        $("#login-submit")[0].disabled = false;
        }
    });
});
