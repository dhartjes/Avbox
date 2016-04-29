$.fn.serializeObject = function() {
    var e = {},
        t = this.serializeArray();
    return $.each(t, function() {
        e[this.name] ? (e[this.name].push || (e[this.name] = [e[this.name]]), e[this.name].push(this.value || "")) : e[this.name] = this.value || ""
    }), e
};
var Parameterize = {
        parameterize: function(e) {
            var t = "-",
                i = e.replace(/[^-\w]+/gi, t);
            return i = i.replace(/-{2,}/gi, t), i = i.replace(/^-|-$/gi, ""), i.toLowerCase()
        }
    },
    FillHeader = {
        _templatesLoaded: !1,
        init: function() {
            _.extend(this, AJAXUtil), this._bindToDOM()
        },
        _bindToDOM: function() {
            var e = $("#templates-dropdown");
            e._on("click", this._onTemplateDropdownClick, this, !0), e._on("mouseover", this._onTemplateDropdownClick, this)
        },
        _onTemplateDropdownClick: function() {
            this._templatesLoaded || (this._templatesLoaded = !0, this.get("/templates", {}, this._doneTemplateDropdownClick))
        },
        _doneTemplateDropdownClick: function(e) {
            $("#pen-templates-dropdown").html(e.html)
        }
    },
    FillActivity = {
        _htmlLoaded: !1,
        init: function() {
            _.extend(this, AJAXUtil), this._bindToDOM()
        },
        _bindToDOM: function() {
            var e = $("#header-activity-div");
            e._on("click", this._onActivityActive, this, !0), e._on("mouseover", this._onActivityActive, this)
        },
        _onActivityActive: function() {
            this._htmlLoaded || (this._htmlLoaded = !0, this.get("/activity/header/", {}, this._doneActivityActive))
        },
        _doneActivityActive: function(e) {
            $("#activity-dropdown").html(e.html)
        }
    };
FillHeader.init(), FillActivity.init();
var SettingsHireable = {
        init: function(e) {
            _.extend(this, AJAXUtil), _.extend(this, e), this.user = __user, this.team = __team, this._bindToDOM()
        },
        _bindToDOM: function() {
            $("#change-hire-email")._on("click", function() {
                $("#nav-profile").click()
            }), $("#hireable-buttons .button")._on("click", this._toggleHireable, this)
        },
        _toggleHireable: function(e, t) {
            $.showMessage(Copy.togglingStatus), this._makeHireButtonAndModuleActive(t), this._saveHireableStatus(t)
        },
        _makeHireButtonAndModuleActive: function(e) {
            $("#" + e.data("profiled") + "-hireable .button").removeClass("active"), e.addClass("active"), "yes" === e.data("value") ? $(".hire-settings").removeClass("hide") : $(".hire-settings").addClass("hide")
        },
        _saveHireableStatus: function(e) {
            var t = this;
            this.post("/" + this.user.username + "/settings/toggle_hireable", this._getToggleHireableParams(e), this._doneToggleHireable, function(i) {
                t.failedToggleHireable(i, e)
            })
        },
        _getToggleHireableParams: function(e) {
            var t = "yes" === e.data("value") ? !0 : !1;
            return {
                hireable: t,
                type: e.data("profiled")
            }
        },
        _doneToggleHireable: function(e) {
            $.showMessage(this._getHireableMessage(e))
        },
        _getHireableMessage: function(e) {
            return e.hireable ? "team" === e.hireable_type ? Copy.yourTeamIsHireable : Copy.youAreHireable : "team" === e.hireable_type ? Copy.yourTeamIsNoLongerHireable : Copy.youAreNoLongerHireable
        },
        failedToggleHireable: function(e, t) {
            $("#" + t.data("profiled") + "-hireable .button").removeClass("active"), $("#" + t.data("profiled") + "-hireable .no-button").addClass("active"), this.showStandardErrorMessage(e)
        }
    },
    Resample = function(e) {
        function t(e, t, n, r) {
            var s = "string" == typeof e,
                o = s || e;
            s && (o = new Image, o.onload = a, o.onerror = i), o._onresample = r, o._width = t, o._height = n, s ? o.src = e : a.call(e)
        }

        function i() {
            throw "not found: " + this.src
        }

        function a() {
            var t = this,
                i = t._width,
                a = t._height,
                s = t._onresample,
                o = Math.min(t.height, t.width);
            null == i && (i = r(t.width * a / t.height)), null == a && (a = r(t.height * i / t.width)), delete t._onresample, delete t._width, delete t._height, e.width = i, e.height = a, n.drawImage(t, 0, 0, o, o, 0, 0, i, a), s(e.toDataURL("image/png"))
        }
        var n = e.getContext("2d"),
            r = Math.round;
        return t
    }(this.document.createElement("canvas")),
    SearchFilter = {
        searchField: $("#assets-search"),
        init: function() {
            this._bindToDOM()
        },
        _bindToDOM: function() {
            this.searchField._on("keyup click search", this._onSearchChange, this, !0)
        },
        _onSearchChange: function(e, t) {
            $(".single-asset").hide(), $(".single-asset .file-name:contains('" + t.val() + "')").closest(".single-asset").show()
        }
    },
    AssetsSign = {
        init: function() {
            _.extend(this, AJAXUtil)
        },
        signAsset: function(e, t, i) {
            var a = this._buildParams(e);
            this.post("/static/sign_asset", a, function(a) {
                AssetsSign.doneSign(a, e, t, i)
            })
        },
        _buildParams: function(e) {
            var t = e.id ? e.id : 0;
            return {
                id: t,
                contentType: e.type,
                fileName: FileUtil.makeNameSafeToSave(e.name),
                fileKB: FileUtil.fileSizeInKb(e),
                template: this._findTemplate()
            }
        },
        _findTemplate: function() {
            return "assets_fullpage" === __pageType ? "asset_fullpage" : "asset"
        },
        signProfileNew: function(e, t) {
            for (var i = {}, a = 0; a < e.length; a++)
                if ("string" == typeof e[a].data) {
                    var n = AssetsSign.dataURItoBlob(e[a].data),
                        r = Math.round(n.size / 1e3);
                    0 === r && (r = 1), e[a] = {
                        dimension: e[a].dimension,
                        size: r,
                        data: n
                    }, i["sizes[" + e[a].dimension + "]"] = r
                }
            this.post("/static/sign_profile/" + t, i, function(t) {
                AssetsSign.doneSignProfile(t, e)
            })
        },
        doneSignProfile: function(e, t) {
            for (var i = 0; i < t.length; i++) AssetsSign.doneSign(e[t[i].dimension], t[i].data, AssetsSign.profileReallyDone)
        },
        profileReallyDone: function() {},
        dataURItoBlob: function(e) {
            for (var t = atob(e.split(",")[1]), i = new ArrayBuffer(t.length), a = new Uint8Array(i), n = 0; n < t.length; n++) a[n] = t.charCodeAt(n);
            return new Blob([i], {
                type: "image/jpeg"
            })
        },
        doneSign: function(e, t, i, a) {
            var n = AssetsSign.getXHRObject(e, i, a),
                r = new FormData;
            for (var s in e.form_data) r.append(s, e.form_data[s]);
            r.append("file", t), n.open("POST", e.url, !0), n.send(r)
        },
        getXHRObject: function(e, t, i) {
            var a = new XMLHttpRequest;
            return a.rslt = e, "withCredentials" in a || (a = null), a.onreadystatechange = function() {
                4 === a.readyState && (201 === a.status ? t(a.rslt) : "undefined" != typeof i && i(a))
            }, a
        }
    };
AssetsSign.init(), jQuery.event.props.push("dataTransfer");
var SettingsAvatar = {
    imageChanger: $(".image-changer"),
    fileInput: $(".file-upload-button"),
    theBody: $("body"),
    init: function() {
        _.extend(this, AssetsSign), this.bindUIActions(), this.imageData = [], this.resizeCount = 0
    },
    bindUIActions: function() {
        this.imageChanger._on("drop", function(e) {
            e.preventDefault(), SettingsAvatar.handleDrop(e)
        }), this.theBody._on("dragover", function(e) {
            e.currentTarget === SettingsAvatar.theBody[0] && SettingsAvatar.testIfContainsFiles(e) && SettingsAvatar.highlightDropArea()
        }), this.theBody._on("dragleave", function(e) {
            e.currentTarget === SettingsAvatar.theBody[0] && SettingsAvatar.unHighlightDropArea()
        }), SettingsAvatar.fileInput._on("change", function(e) {
            SettingsAvatar.handleManualFileSelect(e)
        })
    },
    testIfContainsFiles: function(e) {
        if (e.dataTransfer.types)
            for (var t = 0; t < e.dataTransfer.types.length; t++)
                if ("Files" === e.dataTransfer.types[t]) return !0;
        return !1
    },
    highlightDropArea: function() {
        SettingsAvatar.imageChanger.addClass("drop-here")
    },
    unHighlightDropArea: function() {
        SettingsAvatar.imageChanger.removeClass("drop-here")
    },
    handleDrop: function(e) {
        var t = SettingsAvatar.whichAvatar(e, "team-profile-image-changer"),
            i = e.dataTransfer.files;
        SettingsAvatar.processFile(i, t)
    },
    whichAvatar: function(e, t) {
        return e.currentTarget.id === t ? "team" : "personal"
    },
    handleManualFileSelect: function(e) {
        var t = e.target.files,
            i = SettingsAvatar.whichAvatar(e, "team-profile-image-upload-input");
        SettingsAvatar.processFile(t, i)
    },
    noEmailInAccount: function() {
        $.showMessage(Copy.noAvatarWithoutEmail, "slow")
    },
    processFile: function(e, t) {
        if (__has_email === !1) return this.noEmailInAccount();
        SettingsAvatar.unHighlightDropArea();
        var i = e[0];
        if (i.type.match("image.*"))
            for (var a = 0; 3 > a; a++) 0 === a ? this.resizeImage(i, 20, function(e) {
                SettingsAvatar.imageData.push({
                    dimension: 20,
                    data: e
                }), SettingsAvatar.callSign(t)
            }) : 1 === a ? this.resizeImage(i, 80, function(e) {
                "team" === t ? SettingsAvatar.placeAvatar(e, "#mini-team-avatar") : SettingsAvatar.placeAvatar(e, "#mini-personal-avatar"), SettingsAvatar.imageData.push({
                    dimension: 80,
                    data: e
                }), SettingsAvatar.callSign(t)
            }) : this.resizeImage(i, 512, function(e) {
                "team" === t ? SettingsAvatar.placeAvatar(e, "#team-avatar") : SettingsAvatar.placeAvatar(e, "#personal-avatar"), SettingsAvatar.imageData.push({
                    dimension: 512,
                    data: e
                }), SettingsAvatar.callSign(t)
            });
        else $.showMessage("Oops! That file wasn't an image."), SettingsAvatar.unHighlightDropArea()
    },
    callSign: function(e) {
        2 === SettingsAvatar.resizeCount ? (SettingsAvatar.resizeCount = 0, "team" === e ? ($.showMessage("Team profile avatar saved."), AssetsSign.signProfileNew(SettingsAvatar.imageData, e)) : ($.showMessage("Profile avatar saved."), AssetsSign.signProfileNew(SettingsAvatar.imageData, e))) : ++SettingsAvatar.resizeCount
    },
    resizeImage: function(e, t, i) {
        var a = new FileReader;
        a.onload = function() {
            Resample(this.result, t, t, i)
        }, a.readAsDataURL(e), a.onabort = function() {
            $.showMessage("The upload was aborted.")
        }, a.onerror = function() {
            $.showMessage("An error occured while reading the file.")
        }
    },
    placeAvatar: function(e, t) {
        $(t).attr("src", e)
    }
};
"undefined" == typeof window.Copy && (window.Copy = {}), _.extend(window.Copy, {
    errorSavingSettings: "CodePen couldn't save your settings. See highlighted errors and try again.",
    savingSettingsMessage: "Saving settings",
    settingsSavedMessage: "Settings saved",
    invalidDataMessage: "Validation error, check forms below.",
    savingEmailMessage: "Saving email",
    emailSavedMessage: "Email saved",
    cancelingSubscription: "Canceling Subscription",
    subscriptionCancelled: "Subscription Canceled",
    togglingStatus: "Toggling Status",
    deactivationDate: "Deactivation Date:",
    youAreHireable: "You are now hirable",
    youAreNoLongerHireable: "You're no longer hireable",
    yourTeamIsHireable: "Your team is now hireable",
    yourTeamIsNoLongerHireable: "Your team is no longer hireable",
    cancel: "Cancel",
    updateCreditCardInfo: "Update credit card info",
    noAvatarWithoutEmail: "You can not change your avatar until you've provided a valid email.",
    yourCardUpdated: "Your card was updated.",
    yourPlanUpdated: "Your plan was updated.",
    changingPlan: "Changing your plan",
    yourNameRequired: "Your name is required",
    yourAddressRequired: "Your address is required",
    yourZipRequired: "Your zip is required",
    yourPlanRequired: "A plan is required",
    yourCVCRequired: "Your CVC is required",
    yourCardNumberRequired: "Your CC# is required",
    promoPlanType: "Promo Plan",
    planProMonthly: "Basic PRO Monthly",
    planSuperMonthly: "Super PRO Monthly",
    planProYearly: "Basic PRO Yearly",
    planSuperYearly: "Super PRO Yearly",
    unlinkingFromGitHub: "Unlinking your GitHub account",
    unlinkedFromGitHub: "Your account has been unlinked from GitHub",
    linkingToGitHub: "Linking to your GitHub Account",
    linkedToGitHub: "Your account is now linked to GitHub"
});
var SettingsBioCounter = {
        bio: $("#bio"),
        bioCount: $("#bio-remaining-chars"),
        init: function() {
            this.bindUIEvents(), this.updateCharCount(null, this.bioCount)
        },
        bindUIEvents: function() {
            this.bio._on("keyup", this.updateCharCount)
        },
        updateCharCount: function() {
            SettingsBioCounter.bioCount.text(SettingsBioCounter.bio.val().length)
        }
    },
    SettingsSavePage = {
        init: function(e, t) {
            _.extend(this, AJAXUtil), _.extend(this, e), _.extend(this, t), this.pageName = this.pageName(), this.user = __user, this.team = __team, this._bindToDOM()
        },
        _bindToDOM: function() {
            $(".save-settings-button")._on("click", this.save, this)
        },
        triggerHTML5Validation: function() {
            for (var e = this._getFormElements(), t = !0, i = 0; i < e.length; i++) {
                var a = $(e[i]),
                    n = !0;
                if (a.length && (n = a[0].checkValidity()), !n) {
                    t = !1, a.append("<input type='submit' class='for-validity-test hide'>"), NastyBrowserSniffing.safari || a.find("input.for-validity-test").click().remove();
                    break
                }
            }
            return t
        },
        save: function() {
            var e = this.triggerHTML5Validation();
            return e ? (this.clearErrors(), $.showMessage(Copy.savingSettingsMessage), void this.post("/" + this.user.username + "/settings/save/" + this.pageName, this.postArgs(), this.afterSave, this.saveFailed)) : ($.showMessage(Copy.invalidDataMessage), !1)
        },
        afterSave: function(e) {
            $.showMessage(Copy.settingsSavedMessage, "slow"), void 0 !== this.afterSaveLogic && this.afterSaveLogic(e)
        },
        saveFailed: function(e) {
            return e.errors.email_missing ? this.showErrorModal(e.errors.email_missing[0]) : (this.clearErrors(), void this.showUserErrors(e))
        },
        showErrorModal: function(e) {
            $.showModal("<h1>Settings Saving Error</h1>      <p>" + e + "</p>      <p><a class='button button-medium close-button' href='#0'>Close</a>", "modal-error")
        }
    },
    SettingsSaveProfile = {
        init: function() {},
        pageName: function() {
            return "profile"
        },
        postArgs: function() {
            var e = JSON.stringify(this._getFormValues());
            return {
                profile_settings: e
            }
        },
        _getFormElements: function() {
            return ["#profile-settings-1", "#profile-settings-2", "#profile-settings-3"]
        },
        _getFormValues: function() {
            return $.extend($("#profile-settings-1").serializeObject(), $("#profile-settings-2").serializeObject(), $("#profile-settings-3").serializeObject())
        }
    },
    SettingsCommon = {
        pro: "",
        user: {},
        PLAN_PROMO: "5",
        PLAN_SUPER_PRO_YEARLY: "4",
        PLAN_PRO_YEARLY: "3",
        PLAN_SUPER_PRO_MONTHLY: "2",
        PLAN_PRO_MONTHLY: "1",
        FREE: "0",
        init: function() {
            this.user = __user
        },
        showUserErrors: function(e) {
            this._addErrorsToHTML(e.errors)
        },
        _addErrorsToHTML: function(e, t) {
            for (var i in e) {
                var a = '<div class="error-message">' + e[i][0] + "</div>",
                    n = "#" + this._prepend(t) + i,
                    r = $(n);
                r.length > 0 && $(n).closest("div").append(a).addClass("error")
            }
        },
        _prepend: function(e) {
            return void 0 === e ? "" : e + "-"
        },
        clearErrors: function() {
            $(".error-message").remove(), $(".error").removeClass("error")
        }
    };
SettingsCommon.init(), SettingsSaveProfile.init(), SettingsAvatar.init(), SettingsBioCounter.init(), SettingsSavePage.init(SettingsCommon, SettingsSaveProfile), SettingsHireable.init(SettingsCommon);