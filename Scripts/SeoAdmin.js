(function ($) {
    $.extend(true, {
        onestopSeo: {
            admin: {
                init: function () {
                    $(".onestop-seo-rewrite-value").each(function (index) {
                        var textBox = $(this);
                        var defaultValue = textBox.attr("data-generated-default");
                        var maxLength = textBox.attr("data-max-length");

                        var refreshCounter = function () {
                            var length = textBox.val().length;

                            if (length > maxLength) textBox.addClass("onestop-seo-too-long");
                            else textBox.removeClass("onestop-seo-too-long");

                            textBox.parents("li").first().find(".onestop-seo-character-counter").text(length);
                        };

                        refreshCounter();

                        if (textBox.val() == defaultValue) {
                            textBox.addClass("onestop-seo-generated-default");
                        }

                        textBox.focus(function (e) {
                            if (textBox.val() != defaultValue) return;
                            textBox.removeClass("onestop-seo-generated-default");
                            textBox.val("");
                            refreshCounter();
                        });

                        textBox.blur(function (e) {
                            if (textBox.val() != "" && textBox.val() != defaultValue) return;

                            textBox.addClass("onestop-seo-generated-default");
                            textBox.val(defaultValue);
                            refreshCounter();
                        });

                        textBox.keyup(function (e) {
                            refreshCounter();
                        });
                    });
                }
            }
        }
    });
})(jQuery);