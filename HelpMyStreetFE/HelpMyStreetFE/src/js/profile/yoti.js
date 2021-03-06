import { hmsFetch, fetchResponses } from "../shared/hmsFetch.js";

export async function processYoti(thisToken, userId, mobile) {
    $('#overlay').show();
    $('.loading-overlay').show();
    var response = await hmsFetch("/yoti/ValidateToken?token=" + thisToken + "&u=" + userId,null,{timeOutLength: 10000, timeOutRetry: 0, errorRetry: 0});
    if (response.fetchResponse == fetchResponses.SUCCESS) {
        window.location.href = "/Account";
    } else {
        if (mobile) {
            window.location.href = "/Account?auth=failed";
        } else {
            var event = document.createEvent('Event');
            event.initEvent('failed-auth', true, true);
            document.getElementById('verification-panel').dispatchEvent(event);
            $('#overlay').hide();
            $('.loading-overlay').hide();
        }
    }
}
var yoti = new Object();
    
export function initialiseYoti() {
    if (initObj) {   
        if (yoti.instance) {
            yoti.instance.destroy();
        }
       yoti.instance = window.Yoti.Share.init({
            elements: [
                {
                    domId: initObj.domId,
                    scenarioId: initObj.scenarioId,
                    clientSdkId: initObj.clientSdkId,
                    type: "inline",             
                    qr: {
                        title: "Scan with the Yoti app"
                    },
                    modal: {
                        zIndex: 9999,
                    },
                    shareComplete: {
                        tokenHandler: async (token, done) => {
                            processYoti(token, initObj.userId, false);
                            done();
                        },
                    },
                },
            ],
       });

    }else {
            throw new Error("initObj is null");
        }   
}
