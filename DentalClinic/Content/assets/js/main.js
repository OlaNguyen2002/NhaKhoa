(function ($) {
    "use strict";

    /*====  Document Ready Function =====*/
    jQuery(document).ready(function($){
        //Mobile Menu
        $("#main-menu").slicknav({
            allowParentLinks: true,
            prependTo: '#mobile-menu-wrap',
            label: '',
        });

        $(".mobile-menu-trigger").on("click", function(e) {
            $(".mobile-menu-container").addClass("menu-open");
            e.stopPropagation();
        });

        $(".mobile-menu-close").on("click", function(e) {
            $(".mobile-menu-container").removeClass("menu-open");
            e.stopPropagation();
        });

        //Header Search
        $(".wrd-header-src-btn").on("click", function(e) {
            $(".header-search-wrapper").addClass("search-open");
            e.stopPropagation();
        });

        $(".search-close").on("click", function(e) {
            $(".header-search-wrapper").removeClass("search-open");
            e.stopPropagation();
        });

        // Enable inline Background image
        $("[data-background]").each(function () {
            $(this).css("background-image", "url( " + $(this).attr("data-background") + "  )");
        });

        // Scroll To Top
        $(window).on("scroll",function(){
            var pagescroll = $(window).scrollTop();
            if(pagescroll > 100){
                $(".scroll-to-top").addClass("scroll-to-top-visible");

            }else{
                $(".scroll-to-top").removeClass("scroll-to-top-visible");
            }
        });

        $(".scroll-to-top").click(function() {
            $("html, body").animate({ scrollTop: 0 }, "slow");
            return false;
        });

        // Home slider
        function wrdslider() {
            var SliderActive = $('.wrd-home-slider-wrapper');

            SliderActive.slick({
                slidesToShow: 1,
                autoplay: false,
                autoplaySpeed: 5000,
                speed: 1000, // slide speed
                dots: false,
                fade: true,
                draggable: true,
                pauseOnHover: false,
                arrows: true,
                prevArrow: '<i class="slick-arrow slick-prev flaticon-long-left-arrow"></i>',
                nextArrow: '<i class="slick-arrow slick-next flaticon-long-right-arrow"></i>',
            });


            function doAnimations(elements) {
                var animationEndEvents = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
                elements.each(function () {
                    var $this = $(this);
                    var $animationDelay = $this.data('delay');
                    var $animationType = 'animated ' + $this.data('animation');
                    $this.css({
                        'animation-delay': $animationDelay,
                        '-webkit-animation-delay': $animationDelay
                    });
                    $this.addClass($animationType).one(animationEndEvents, function () {
                        $this.removeClass($animationType);
                    });
                });
            }

            SliderActive.on('init', function (e, slick) {
                var $firstAnimatingElements = $('.wrd-single-slide-item:first-child').find('[data-animation]');
                doAnimations($firstAnimatingElements);
            });

            SliderActive.on('beforeChange', function (e, slick, currentSlide, nextSlide) {
                var $animatingElements = $('.wrd-single-slide-item[data-slick-index="' + nextSlide + '"]').find('[data-animation]');
                doAnimations($animatingElements);
            });

        }
        wrdslider();

        //$(".wrd-count-number").counterUp({
        //    delay: 10,
        //    time: 2000
        //});

        // Team Slider
        $("#wrd-doctors-one-slider").slick({
            slidesToShow: 3,
            autoplay: false,
            autoplaySpeed: "4000", //interval
            speed: 1500, // slide speed
            dots: false,
            arrows: true,
            prevArrow: '<i class="slick-arrow slick-prev flaticon-long-left-arrow"></i>',
            nextArrow: '<i class="slick-arrow slick-next flaticon-long-right-arrow"></i>',
            infinite:  true,
            pauseOnHover: false,
            centerMode: false,
            rows: 1,
            responsive: [
                {
                    breakpoint: 1025,
                    settings: {
                        slidesToShow: 3, // 992-1024
                        arrows: false,
                    }
                },
                {
                    breakpoint: 992,
                    settings: {
                        slidesToShow: 3, //768-991
                        arrows: false,
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 1, // 0 -767
                        arrows: false,
                    }
                }
            ]
        });

        // Team Slider
        $("#wrd-doctors-two-slider").slick({
            slidesToShow: 4,
            autoplay: false,
            autoplaySpeed: "4000", //interval
            speed: 1500, // slide speed
            dots: false,
            arrows: false,
            prevArrow: '<i class="slick-arrow slick-prev flaticon-long-left-arrow"></i>',
            nextArrow: '<i class="slick-arrow slick-next flaticon-long-right-arrow"></i>',
            infinite:  true,
            pauseOnHover: false,
            centerMode: false,
            rows: 1,
            responsive: [
                {
                    breakpoint: 1025,
                    settings: {
                        slidesToShow: 3, // 992-1024
                        arrows: false,
                    }
                },
                {
                    breakpoint: 992,
                    settings: {
                        slidesToShow: 2, //768-991
                        arrows: false,
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 1, // 0 -767
                        arrows: false,
                    }
                }
            ]
        });

        // Testimonial Slider
        $("#wrd-testimonial-slider-one").slick({
            slidesToShow: 2,
            autoplay: true,
            autoplaySpeed: "4000", //interval
            speed: 1500, // slide speed
            dots: true,
            arrows: true,
            prevArrow: '<i class="slick-arrow slick-prev flaticon-long-left-arrow"></i>',
            nextArrow: '<i class="slick-arrow slick-next flaticon-long-right-arrow"></i>',
            infinite:  true,
            pauseOnHover: false,
            centerMode: false,
            responsive: [
                {
                    breakpoint: 1025,
                    settings: {
                        slidesToShow: 2, // 992-1024
                        arrows: false,
                    }
                },
                {
                    breakpoint: 992,
                    settings: {
                        slidesToShow: 1, //768-991
                        arrows: false,
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 1, // 0 -767
                        arrows: false,
                    }
                }
            ]
        });

        // Testimonial Two Slider
        $(".wrd-testimonial-two-slider").slick({
            slidesToShow: 3,
            autoplay: true,
            autoplaySpeed: "4000", //interval
            speed: 1500, // slide speed
            dots: false,
            arrows: false,
            prevArrow: '<i class="slick-arrow slick-prev flaticon-long-left-arrow"></i>',
            nextArrow: '<i class="slick-arrow slick-next flaticon-long-right-arrow"></i>',
            infinite:  true,
            pauseOnHover: false,
            vertical: true,
            verticalSwiping: true,
            responsive: [
                {
                    breakpoint: 1025,
                    settings: {
                        slidesToShow: 2, // 992-1024
                        vertical: false,
                        verticalSwiping: false,
                    }
                },
                {
                    breakpoint: 992,
                    settings: {
                        slidesToShow: 2, //768-991
                        vertical: false,
                        verticalSwiping: false,
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 1, // 0 -767
                        vertical: false,
                        verticalSwiping: false,
                    }
                }
            ]
        });

        // Testimonial Three Slider
        $(".wrd-testimonial-three").slick({
            slidesToShow: "1",
            autoplay: true,
            autoplaySpeed: "4000", //interval
            speed: 1500, // slide speed
            dots: true,
            arrows: false,
            prevArrow: '<i class="slick-arrow slick-prev flaticon-long-left-arrow"></i>',
            nextArrow: '<i class="slick-arrow slick-next flaticon-long-right-arrow"></i>',
            infinite:  true,
            pauseOnHover: false,
            centerMode: false,
            responsive: [
                {
                    breakpoint: 1025,
                    settings: {
                        slidesToShow: 2, // 992-1024
                        arrows: false,
                    }
                },
                {
                    breakpoint: 992,
                    settings: {
                        slidesToShow: "2", //768-991
                        arrows: false,
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 1, // 0 -767
                        arrows: false,
                    }
                }
            ]
        });


        // Brand Slider
        $(".wrd-brand-image-slider").slick({
            slidesToShow: 5,
            autoplay: true,
            autoplaySpeed: "4000", //interval
            speed: 1500, // slide speed
            dots: false,
            arrows: false,
            prevArrow: '<i class="slick-arrow slick-prev eicon-chevron-left"></i>',
            nextArrow: '<i class="slick-arrow slick-next eicon-chevron-right"></i>',
            infinite: true,
            pauseOnHover: false,
            centerMode: false,
            responsive: [
                {
                    breakpoint: 1025,
                    settings: {
                        slidesToShow: 4, // 992-1024
                        arrows: false,
                    }
                },
                {
                    breakpoint: 992,
                    settings: {
                        slidesToShow: 4, //768-991
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 2, // 0 -767
                    }
                }
            ]
        });

        //Icon Box Slider
        $(".wrd-icon-box-slider").slick({
            slidesToShow: 4,
            autoplay: true,
            autoplaySpeed: "6000", //interval
            speed: 1500, // slide speed
            dots: false,
            arrows: false,
            prevArrow: '<i class="slick-arrow slick-prev eicon-chevron-left"></i>',
            nextArrow: '<i class="slick-arrow slick-next eicon-chevron-right"></i>',
            infinite: true,
            pauseOnHover: false,
            centerMode: false,
            responsive: [
                {
                    breakpoint: 1025,
                    settings: {
                        slidesToShow: 3, // 992-1024
                        arrows: false,
                    }
                },
                {
                    breakpoint: 992,
                    settings: {
                        slidesToShow: 2, //768-991
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 1, // 0 -767
                    }
                }
            ]
        });


        // Service Details slider
        $(".wrd-service-details-main-slider").slick({
            slidesToShow: 1,
            slidesToScroll: 1,
            autoplay: true,
            autoplaySpeed: 4000, //interval
            speed: 1500, // slide speed
            dots: false,
            arrows: false,
            infinite: true,
            fade: true,
            pauseOnHover: false,
            centerMode: false,
            asNavFor: '.wrd-service-details-slider-nav'
        });

        $(".wrd-service-details-slider-nav").slick({
            slidesToShow:  3,
            slidesToScroll: 1,
            autoplay: true,
            autoplaySpeed: 4000, //interval
            speed: 1500, // slide speed
            dots: false,
            arrows: false,
            infinite: true,
            pauseOnHover: false,
            centerMode: true,
            centerPadding: '0px',
            asNavFor: '.wrd-service-details-main-slider',
            focusOnSelect: true,
            responsive: [
                {
                    breakpoint: 992,
                    settings: {
                        slidesToShow: 3, //768-991
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 3, // 0 -767
                    }
                }
            ]
        });





        // Masonry Gallery
        $(".wrd-masonry-item-wrapper").imagesLoaded(function () {
            $(".wrd-masonry-item-wrapper").isotope({
                itemSelector: ".single-wrd-masonry-item",
                percentPosition: true,
                transitionDuration: '.8s',
                masonry: {
                    columnWidth: 1
                }
            });
        });


        // Popup Image
        $('.wrd-popup-image').magnificPopup({
            type: 'image',
            gallery: {
                enabled: true
            }
        });

        // Popup Video
        $(".wrd-video-button").magnificPopup({
            type: 'video'
        });


        // Buy Count Button
        function incrementValue(e) {
            e.preventDefault();
            var fieldName = $(e.target).data('field');
            var parent = $(e.target).closest('div');
            var currentVal = parseInt(parent.find('input[name=' + fieldName + ']').val(), 10);

            if (!isNaN(currentVal)) {
                parent.find('input[name=' + fieldName + ']').val(currentVal + 1);
            } else {
                parent.find('input[name=' + fieldName + ']').val(0);
            }
        }

        function decrementValue(e) {
            e.preventDefault();
            var fieldName = $(e.target).data('field');
            var parent = $(e.target).closest('div');
            var currentVal = parseInt(parent.find('input[name=' + fieldName + ']').val(), 10);

            if (!isNaN(currentVal) && currentVal != 1) {
                parent.find('input[name=' + fieldName + ']').val(currentVal - 1);
            } else {
                parent.find('input[name=' + fieldName + ']').val(1);
            }
        }

        $('.number-input-group').on('click', '.button-plus', function(e) {
            incrementValue(e);
        });

        $('.number-input-group').on('click', '.button-minus', function(e) {
            decrementValue(e);
        });

        // Post Print
        $(document).on('click', '.print-button', function(e){
            console.log();
            e.preventDefault();
            window.print();
            return false;
        });

    });

    /*====  Window Load Function =====*/
    jQuery(window).on('load', function() {
        //Preloader
        $('.preloader-wrapper').delay(1000).fadeOut('slow');
        setTimeout(function() {
            $('.site').addClass('loaded');
        }, 500);
    });

}(jQuery));
function slideToggle(t, e, o) { 0 === t.clientHeight ? j(t, e, o, !0) : j(t, e, o) } function slideUp(t, e, o) { j(t, e, o) } function slideDown(t, e, o) { j(t, e, o, !0) } function j(t, e, o, i) { void 0 === e && (e = 400), void 0 === i && (i = !1), t.style.overflow = "hidden", i && (t.style.display = "block"); var p, l = window.getComputedStyle(t), n = parseFloat(l.getPropertyValue("height")), a = parseFloat(l.getPropertyValue("padding-top")), s = parseFloat(l.getPropertyValue("padding-bottom")), r = parseFloat(l.getPropertyValue("margin-top")), d = parseFloat(l.getPropertyValue("margin-bottom")), g = n / e, y = a / e, m = s / e, u = r / e, h = d / e; window.requestAnimationFrame(function l(x) { void 0 === p && (p = x); var f = x - p; i ? (t.style.height = g * f + "px", t.style.paddingTop = y * f + "px", t.style.paddingBottom = m * f + "px", t.style.marginTop = u * f + "px", t.style.marginBottom = h * f + "px") : (t.style.height = n - g * f + "px", t.style.paddingTop = a - y * f + "px", t.style.paddingBottom = s - m * f + "px", t.style.marginTop = r - u * f + "px", t.style.marginBottom = d - h * f + "px"), f >= e ? (t.style.height = "", t.style.paddingTop = "", t.style.paddingBottom = "", t.style.marginTop = "", t.style.marginBottom = "", t.style.overflow = "", i || (t.style.display = "none"), "function" == typeof o && o()) : window.requestAnimationFrame(l) }) }

let sidebarItems = document.querySelectorAll('.sidebar-item.has-sub');
for (var i = 0; i < sidebarItems.length; i++) {
    let sidebarItem = sidebarItems[i];
    sidebarItems[i].querySelector('.sidebar-link').addEventListener('click', function (e) {
        e.preventDefault();

        let submenu = sidebarItem.querySelector('.submenu');
        if (submenu.classList.contains('active')) submenu.style.display = "block"

        if (submenu.style.display == "none") submenu.classList.add('active')
        else submenu.classList.remove('active')
        slideToggle(submenu, 300)
    })
}