(function ($) {
    $.extend(true, {
        onestopSeo: {
            admin: {
                init: function () {
                    $(".onestop-seo-generated-default").each(function (index) {
                        var defaultValue = $(this).val();
                        $(this).attr("data-generated-default", defaultValue);

                        $(this).focus(function () {
                            var textBox = $(this);
                            if (textBox.val() != defaultValue) return;
                            textBox.removeClass("onestop-seo-generated-default");
                            textBox.val("");
                        });

                        $(this).blur(function () {
                            var textBox = $(this);
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