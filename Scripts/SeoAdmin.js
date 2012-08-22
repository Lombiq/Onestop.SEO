(function ($) {
    $.extend(true, {
        onestopSeo: {
            admin: {
                init: function () {
                    $(".onestop-seo-rewrite-value").each(function (index) {
                        var textBox = $(this);
                        var defaultValue = textBox.attr("data-generated-default");
                        var characterCounter = textBox.next(".onestop-seo-character-counter");

                        characterCounter.val(textBox.val().length);

                        if (textBox.val() == defaultValue) {
                            textBox.addClass("onestop-seo-generated-default");
                        }

                        textBox.focus(function (e) {
                            if (textBox.val() != defaultValue) return;
                            textBox.removeClass("onestop-seo-generated-default");
                            textBox.val("");
                        });

                        textBox.blur(function (e) {
                            if (textBox.val() != "" && textBox.val() != defaultValue) return;
                            textBox.addClass("onestop-seo-generated-default");
                            textBox.val(defaultValue);
                        });

                        textBox.keyup(function (e) {
                            var length = textBox.val().length;
                            var maxLength = textBox.attr("data-max-length");

                            if (length > maxLength) textBox.addClass("onestop-seo-too-long");
                            else textBox.removeClass("onestop-seo-too-long");

                            characterCounter.val(length);
                        });
                    });
                }
            }
        }
    });
})(jQuery);