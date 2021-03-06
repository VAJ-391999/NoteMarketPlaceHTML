/* ====================================
              Login Password
====================================== */

$(document).ready(function() {
    $(".login-btn").click(function(){
        var pass = $("#password1").val();
        
        if (pass == "admin") {
            
        }else {
            $(".incorrect-password").css("display", "block");
            $("#form-content input[type=password]").css("border-color","#ff5e5e");
        }
    });
});

$(function() {
   $("#togglepassword").on('click', function() {
       var passfield = $("#password1");
       var passFieldType = passfield.attr('type');
       if (passFieldType == "password") {
           
           passfield.attr('type', 'text');
           
       }else {
           passfield.attr('type', 'password');
       }
   }); 
});

/* ====================================
               Change Password
====================================== */

$(function() {
   $("#togglepassword1").on('click', function() {
       var passfield = $("#oldpassword");
       var passFieldType = passfield.attr('type');
       if (passFieldType == "password") {
           
           passfield.attr('type', 'text');
           
       }else {
           passfield.attr('type', 'password');
       }
   }); 
    
});

$(function() {
   $("#togglepassword2").on('click', function() {
       var passfield = $("#newpassword");
       var passFieldType = passfield.attr('type');
       if (passFieldType == "password") {
           
           passfield.attr('type', 'text');
           
       }else {
           passfield.attr('type', 'password');
       }
   }); 
    
});

$(function() {
   $("#togglepassword3").on('click', function() {
       var passfield = $("#confirmpassword");
       var passFieldType = passfield.attr('type');
       if (passFieldType == "password") {
           
           passfield.attr('type', 'text');
           
       }else {
           passfield.attr('type', 'password');
       }
   }); 
    
});

/* ====================================
               Sign Up 
====================================== */
$(function(){
   $(".signup-btn").on('click', function() {
       $(".success-message").css("display", "block");
   });
});

/* ====================================
               Navigation
====================================== */

/* Show And Hide White Navigation */

$(function () {
    
    //Show/Hide nav on page load
    showHideNav ();
    
    $(window).scroll(function(){
        
        //Show/Hide nav on window's scroll
        showHideNav ();
     
    });
    
    function showHideNav () {
        
        if( $(window).scrollTop() > 50 ) {
            
            //Show White Nav
            $("#home-header nav").addClass("white-nav-top");
            
            //Show Dark logo
            $("#home-header .navbar-brand img").attr("src", "images/pre-login/top-purple-logo.jpeg");
            
            //Show Back to top button
            //$("#back-to-top").fadeIn();
           
        } else {
            
            //Show White Nav
            $("#home-header nav").removeClass("white-nav-top");
           
            //Show logo
            $("#home-header .navbar-brand img").attr("src", "images/pre-login/top-logo.png");
            
            //Hide Back to top button
            //$("#back-to-top").fadeOut();
        }
        
    }

});

//Smooth Scrolling 

$(function () {
    
    $("a.smooth-scroll").click(function(event) {
        
        event.preventDefault();
        
        // get section id like #about, #services, #work, #team and etc.
        var section_id = $(this).attr("href");
        
        $("html, body").animate({
            scrollTop: $(section_id).offset().top - 64
        }, 1250, "easeInOutExpo");
        
    });
    
});

/* ==================================
            Mobile Menu
====================================*/
$(function () {

    // Show Mobile nav
    $("#mobile-nav-open-btn").click(function () {
        $("#mobile-nav").css("height", "100%");
    });

    // Hide Mobile nav
    $("#mobile-nav #mobile-nav-close-btn, #mobile-nav a").click(function () {
        $("#mobile-nav").css("height", "0");
    });


});

/* ====================================
               Button
====================================== */

$( ".download-btn" ).click(function() {
    $( ".download-btn" ).css('background', '#6255a5');
  });


/* ====================================
               Test
====================================== */
$(function(){
   
    $(".accordion-content").hide();
    $("#accordion-button-1").on('click', () =>{
       $("#accordion-content-1") .toggle();
        $("#up1").toggleClass('rotate');
    });
    
    $("#accordion-button-2").on('click', () =>{
       $("#accordion-content-2") .toggle();
        $("#up2").toggleClass('rotate');
    });
    
    $("#accordion-button-3").on('click', () =>{
       $("#accordion-content-3") .toggle();
        $("#up3").toggleClass('rotate');
    });
    
    
});












