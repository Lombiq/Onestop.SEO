(function ($) {
    $.extend(true, {
        onestopSeo: {
            admin: {
                init: function () {
                    $(".onestop-seo-rewrite-value").each(function (index) {
                        var textBox = $(this);
                        var defaultValue = textBox.attr("data-generated-default");

                        if (textBox.val() == defaultValue) {
                            textBox.addClass("onestop-seo-generated-default");
                        }

                        $(this).focus(function () {
                            if (textBox.val() != defaultValue) return;
                            textBox.removeClass("onestop-seo-generated-default");
                            textBox.val("");
                        });

                        $(this).blur(function () {
                            if (textBox.val() != "" && textBox.val() != defaultValue) return;
                            textBox.addClass("onestop-seo-generated-default");
                            textBox.val(defaultValue);
                        });
                    });
                }
            }
        }
    });
})(jQuery);