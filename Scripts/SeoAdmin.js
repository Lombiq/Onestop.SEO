(function ($) {
    $.extend(true, {
        onestopSeo: {
            admin: {
                init: function () {
                    $(".onestop-seo-rewrite-value").each(function (index) {
                        var textBox = $(this);
                        var defaultValue = textBox.attr("data-generated-default");
                        var maxLength = textBox.attr("data-max-length");
                        var controls = textBox.parents("li").first().find(".onestop-seo-override-controls");

                        var refreshCounter = function () {
                            var length = textBox.val().length;

                            if (length > maxLength) textBox.addClass("onestop-seo-too-long");
                            else textBox.removeClass("onestop-seo-too-long");

                            controls.find(".onestop-seo-character-counter").text(length);
                        };
                        
                        var indicateDefault = function () {
                            controls.find(".onestop-seo-override-indicate-overridden").hide();
                            controls.find(".onestop-seo-override-indicate-default").show();
                        };
                        
                        var indicateOverridden = function () {
                            controls.find(".onestop-seo-override-indicate-default").hide();
                            controls.find(".onestop-seo-override-indicate-overridden").show();
                        };

                        refreshCounter();

                        if (textBox.val() == defaultValue) indicateDefault();
                        else indicateOverridden();

                        textBox.focus(function (e) {
                            if (textBox.val() != defaultValue) return;
                            textBox.val("");
                            refreshCounter();
                        });

                        textBox.blur(function (e) {
                            if (textBox.val() != "" && textBox.val() != defaultValue) {
                                indicateOverridden();
                                return;
                            }

                            indicateDefault();
                            textBox.val(defaultValue);
                            refreshCounter();
                        });

                        textBox.keyup(function (e) {
                            indicateOverridden();
                            refreshCounter();
                        });
                    });
                }
            }
        }
    });
})(jQuery);