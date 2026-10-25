/* ============================================ */
/* AUTH MODAL FUNCTIONS (GLOBAL)                */
/* ============================================ */

function openLoginModal() {
    var modal = document.getElementById('loginModal');
    if (!modal) return;
    modal.style.display = 'flex';
    document.body.classList.add('modal-open');
    var err = document.getElementById('loginError');
    if (err) err.style.display = 'none';
    setTimeout(function () {
        var emailInput = document.getElementById('loginEmail');
        if (emailInput) emailInput.focus();
    }, 100);
}

function closeLoginModal() {
    var modal = document.getElementById('loginModal');
    if (modal) modal.style.display = 'none';
    document.body.classList.remove('modal-open');
    var form = document.getElementById('loginForm');
    if (form) form.reset();
    var err = document.getElementById('loginError');
    if (err) err.style.display = 'none';
}

function openRegisterModal() {
    var modal = document.getElementById('registerModal');
    if (!modal) return;
    modal.style.display = 'flex';
    document.body.classList.add('modal-open');
    var err = document.getElementById('registerError');
    var succ = document.getElementById('registerSuccess');
    if (err) err.style.display = 'none';
    if (succ) succ.style.display = 'none';
    setTimeout(function () {
        var nameInput = document.getElementById('registerFullName');
        if (nameInput) nameInput.focus();
    }, 100);
}

function closeRegisterModal() {
    var modal = document.getElementById('registerModal');
    if (modal) modal.style.display = 'none';
    document.body.classList.remove('modal-open');
    var form = document.getElementById('registerForm');
    if (form) form.reset();
    var err = document.getElementById('registerError');
    var succ = document.getElementById('registerSuccess');
    if (err) err.style.display = 'none';
    if (succ) succ.style.display = 'none';
}

function switchToRegister() {
    closeLoginModal();
    openRegisterModal();
}

function switchToLogin() {
    closeRegisterModal();
    openLoginModal();
}

/* ============================================ */
/* GENERAL INQUIRY MODAL                        */
/* ============================================ */

function openInquiryModal() {
    var modal = document.getElementById('inquiryModal');
    if (!modal) return;
    modal.style.display = 'flex';
    document.body.classList.add('modal-open');

    // Hide messages
    document.getElementById('inquiryError').style.display = 'none';
    document.getElementById('inquirySuccess').style.display = 'none';

    setTimeout(function () {
        var nameInput = document.getElementById('inquiryName');
        if (nameInput) nameInput.focus();
    }, 100);
}

function closeInquiryModal() {
    var modal = document.getElementById('inquiryModal');
    if (modal) modal.style.display = 'none';
    document.body.classList.remove('modal-open');
    var form = document.getElementById('inquiryForm');
    if (form) form.reset();
}

async function submitGeneralInquiry(e) {
    e.preventDefault();

    var btn = document.getElementById('inquirySubmitBtn');
    var errorBox = document.getElementById('inquiryError');
    var successBox = document.getElementById('inquirySuccess');

    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Sending...';
    errorBox.style.display = 'none';
    successBox.style.display = 'none';

    try {
        var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        var token = tokenInput ? tokenInput.value : '';

        var formData = new FormData();
        // NOTE: We use PropertyId = 0 and generic names for the general inquiry
        formData.append('PropertyId', '0');
        formData.append('PropertyTitle', 'General Inquiry');
        formData.append('AgentName', 'RichPaul Leasing Admin');
        formData.append('AgentEmail', 'leasing@richpaulleasing.com'); // Change to your actual admin email
        formData.append('UserName', document.getElementById('inquiryName').value);
        formData.append('UserEmail', document.getElementById('inquiryEmail').value);
        formData.append('PhoneNumber', document.getElementById('inquiryPhone').value);
        formData.append('Message', document.getElementById('inquiryMessage').value);
        formData.append('__RequestVerificationToken', token);

        // We post to the same SendInquiry action
        var response = await fetch('/Property/SendInquiry', {
            method: 'POST',
            body: formData,
            redirect: 'manual'
        });

        // Check if it succeeded (usually returns a 302 redirect back)
        if (response.type === 'opaqueredirect' || response.status === 0 || response.ok || response.status === 302) {
            successBox.textContent = 'Your inquiry has been sent successfully!';
            successBox.style.display = 'block';
            document.getElementById('inquiryForm').reset();

            setTimeout(function () {
                closeInquiryModal();
            }, 2000);
        } else {
            errorBox.textContent = 'Failed to send inquiry. Please try again.';
            errorBox.style.display = 'block';
        }
    } catch (err) {
        errorBox.textContent = 'An error occurred. Please try again.';
        errorBox.style.display = 'block';
        console.error(err);
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="fas fa-paper-plane"></i> Send Inquiry';
    }
}

/* ============================================ */
/* SUBMIT LOGIN VIA AJAX (GLOBAL)               */
/* ============================================ */

async function submitLogin(e) {
    e.preventDefault();

    var btn = document.getElementById('loginSubmitBtn');
    var errorBox = document.getElementById('loginError');

    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Signing in...';
    errorBox.style.display = 'none';

    try {
        var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        var token = tokenInput ? tokenInput.value : '';

        var formData = new FormData();
        formData.append('Input.Email', document.getElementById('loginEmail').value);
        formData.append('Input.Password', document.getElementById('loginPassword').value);
        formData.append('Input.RememberMe', 'false');
        formData.append('__RequestVerificationToken', token);

        var response = await fetch('/Identity/Account/Login', {
            method: 'POST',
            body: formData,
            redirect: 'manual'
        });

        if (response.type === 'opaqueredirect' || response.status === 0 || response.ok) {
            window.location.reload();
            return;
        }

        var html = await response.text();
        var match = html.match(/<div[^>]*class="[^"]*validation-summary-errors[^"]*"[^>]*>([\s\S]*?)<\/div>/);
        var errorMsg = match ? stripHtml(match[1]) : 'Invalid email or password.';
        showLoginError(errorMsg);
    } catch (err) {
        showLoginError('Unable to sign in. Please try again.');
        console.error(err);
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="fas fa-sign-in-alt"></i> Sign In';
    }
}

function showLoginError(msg) {
    var errorBox = document.getElementById('loginError');
    errorBox.textContent = msg;
    errorBox.style.display = 'block';
}

/* ============================================ */
/* CHANGE PASSWORD MODAL                        */
/* ============================================ */

function openChangePasswordModal() {
    var modal = document.getElementById('changePasswordModal');
    if (!modal) return;
    modal.style.display = 'flex';
    document.body.classList.add('modal-open');
    document.getElementById('passwordError').style.display = 'none';
    document.getElementById('passwordSuccess').style.display = 'none';
    document.getElementById('changePasswordForm').reset();
}

function closeChangePasswordModal() {
    var modal = document.getElementById('changePasswordModal');
    if (modal) modal.style.display = 'none';
    document.body.classList.remove('modal-open');
}

async function submitChangePassword(e) {
    e.preventDefault();

    var btn = document.getElementById('passwordSubmitBtn');
    var errorBox = document.getElementById('passwordError');
    var successBox = document.getElementById('passwordSuccess');

    var currentPwd = document.getElementById('currentPassword').value;
    var newPwd = document.getElementById('newPassword').value;
    var confirmPwd = document.getElementById('confirmPassword').value;

    // Client-side validation
    if (newPwd !== confirmPwd) {
        errorBox.textContent = 'New passwords do not match.';
        errorBox.style.display = 'block';
        return;
    }

    if (newPwd.length < 6) {
        errorBox.textContent = 'Password must be at least 6 characters.';
        errorBox.style.display = 'block';
        return;
    }

    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Updating...';
    errorBox.style.display = 'none';
    successBox.style.display = 'none';

    try {
        var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        var token = tokenInput ? tokenInput.value : '';

        var formData = new FormData();
        formData.append('currentPassword', currentPwd);
        formData.append('newPassword', newPwd);
        formData.append('__RequestVerificationToken', token);

        var response = await fetch('/Account/ChangePassword', {
            method: 'POST',
            body: formData
        });

        var data = await response.json();

        if (data.success) {
            successBox.textContent = data.message || 'Password updated successfully!';
            successBox.style.display = 'block';
            document.getElementById('changePasswordForm').reset();

            setTimeout(function () {
                closeChangePasswordModal();
            }, 2000);
        } else {
            errorBox.textContent = data.message || 'Failed to update password.';
            errorBox.style.display = 'block';
        }
    } catch (err) {
        errorBox.textContent = 'An error occurred. Please try again.';
        errorBox.style.display = 'block';
        console.error(err);
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="fas fa-save"></i> Update Password';
    }
}

/* ============================================ */
/* SUBMIT REGISTER VIA AJAX (GLOBAL)            */
/* ============================================ */

async function submitRegister(e) {
    e.preventDefault();

    var btn = document.getElementById('registerSubmitBtn');
    var errorBox = document.getElementById('registerError');
    var successBox = document.getElementById('registerSuccess');

    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Registering...';
    errorBox.style.display = 'none';
    successBox.style.display = 'none';

    try {
        var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        var token = tokenInput ? tokenInput.value : '';

        var formData = new FormData();
        formData.append('Input.Email', document.getElementById('registerEmail').value);
        formData.append('Input.Password', document.getElementById('registerPassword').value);
        formData.append('Input.ConfirmPassword', document.getElementById('registerPassword').value);
        formData.append('Input.FullName', document.getElementById('registerFullName').value);
        formData.append('__RequestVerificationToken', token);

        var response = await fetch('/Identity/Account/Register', {
            method: 'POST',
            body: formData,
            redirect: 'manual'
        });

        if (response.type === 'opaqueredirect' || response.status === 0 || response.ok) {
            successBox.textContent = 'Account created! You can now sign in.';
            successBox.style.display = 'block';

            setTimeout(function () {
                switchToLogin();
                document.getElementById('loginEmail').value = document.getElementById('registerEmail').value;
            }, 1200);
            return;
        }

        var html = await response.text();
        var match = html.match(/<div[^>]*class="[^"]*validation-summary-errors[^"]*"[^>]*>([\s\S]*?)<\/div>/);
        var errorMsg = match ? stripHtml(match[1]) : 'Registration failed. Please try again.';
        errorBox.textContent = errorMsg;
        errorBox.style.display = 'block';
    } catch (err) {
        errorBox.textContent = 'Unable to register. Please try again.';
        errorBox.style.display = 'block';
        console.error(err);
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="fas fa-user-plus"></i> Register';
    }
}

/* ============================================ */
/* PASSWORD VISIBILITY TOGGLE                   */
/* ============================================ */

function togglePassword(inputId, button) {
    var input = document.getElementById(inputId);
    if (!input) return;

    var icon = button.querySelector('i');

    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    }
}

function stripHtml(html) {
    var tmp = document.createElement('div');
    tmp.innerHTML = html;
    return tmp.textContent || tmp.innerText || '';
}

/* ============================================ */
/* CLOSE MODAL ON OUTSIDE CLICK / ESC           */
/* ============================================ */

document.addEventListener('click', function (e) {
    if (e.target.id === 'loginModal') closeLoginModal();
    if (e.target.id === 'registerModal') closeRegisterModal();
});

document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        closeLoginModal();
        closeRegisterModal();
    }
});

/* ============================================ */
/* CAROUSEL (single DOMContentLoaded)           */
/* ============================================ */

document.addEventListener('DOMContentLoaded', function () {
    var track = document.getElementById('carouselTrack');
    if (!track) return; // no carousel on this page

    var slides = document.querySelectorAll('.carousel-slide');
    var prevBtn = document.getElementById('carouselPrev');
    var nextBtn = document.getElementById('carouselNext');
    var dotsContainer = document.getElementById('carouselDots');

    var currentIndex = 0;
    var intervalId = null;
    var totalSlides = slides.length;

    function getVisibleSlides() {
        if (window.innerWidth <= 768) return 1;
        if (window.innerWidth <= 992) return 2;
        return 3;
    }

    var slidesToShow = getVisibleSlides();

    function createDots() {
        var totalDots = Math.max(1, totalSlides - slidesToShow + 1);
        dotsContainer.innerHTML = '';
        for (var i = 0; i < totalDots; i++) {
            var dot = document.createElement('span');
            dot.className = 'dot' + (i === 0 ? ' active' : '');
            dot.dataset.index = i;
            dot.addEventListener('click', function () {
                stopAutoPlay();
                goToSlide(parseInt(this.dataset.index));
                startAutoPlay();
            });
            dotsContainer.appendChild(dot);
        }
    }

    createDots();
    var dots = document.querySelectorAll('.dot');

    function goToSlide(index) {
        var maxIdx = totalSlides - slidesToShow;
        if (index < 0) index = maxIdx;
        if (index > maxIdx) index = 0;

        currentIndex = index;
        var slideWidth = 100 / slidesToShow;
        var offset = -currentIndex * slideWidth;
        track.style.transform = 'translateX(' + offset + '%)';

        dots = document.querySelectorAll('.dot');
        dots.forEach(function (dot, i) {
            dot.classList.toggle('active', i === currentIndex);
        });
    }

    function nextSlide() { goToSlide(currentIndex + 1); }
    function prevSlide() { goToSlide(currentIndex - 1); }

    function startAutoPlay() {
        if (intervalId) clearInterval(intervalId);
        intervalId = setInterval(nextSlide, 3000);
    }

    function stopAutoPlay() {
        if (intervalId) {
            clearInterval(intervalId);
            intervalId = null;
        }
    }

    if (nextBtn) {
        nextBtn.addEventListener('click', function () {
            stopAutoPlay();
            nextSlide();
            startAutoPlay();
        });
    }

    if (prevBtn) {
        prevBtn.addEventListener('click', function () {
            stopAutoPlay();
            prevSlide();
            startAutoPlay();
        });
    }

    var carouselWrapper = document.querySelector('.carousel-wrapper');
    if (carouselWrapper) {
        carouselWrapper.addEventListener('mouseenter', stopAutoPlay);
        carouselWrapper.addEventListener('mouseleave', startAutoPlay);
    }

    var resizeTimeout;
    window.addEventListener('resize', function () {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(function () {
            var newSlidesToShow = getVisibleSlides();
            if (newSlidesToShow !== slidesToShow) {
                slidesToShow = newSlidesToShow;
                createDots();
                dots = document.querySelectorAll('.dot');
                goToSlide(0);
            }
        }, 250);
    });

    goToSlide(0);
    startAutoPlay();
});

/* ============================================ */
/* PROPERTY GALLERY                             */
/* ============================================ */

var galleryCurrentIndex = 0;
var galleryTotalSlides = 0;
var galleryAutoPlayInterval = null;

function galleryInit() {
    var slides = document.querySelectorAll('.gallery-slide');
    galleryTotalSlides = slides.length;
    galleryCurrentIndex = 0;

    if (galleryTotalSlides <= 1) return;

    // Auto-play every 6 seconds
    if (galleryAutoPlayInterval) clearInterval(galleryAutoPlayInterval);
    galleryAutoPlayInterval = setInterval(galleryNext, 6000);
}

function galleryGoTo(index) {
    var slides = document.querySelectorAll('.gallery-slide');
    var thumbs = document.querySelectorAll('.gallery-thumb');
    var counter = document.getElementById('galleryCurrent');

    if (slides.length === 0) return;

    if (index < 0) index = slides.length - 1;
    if (index >= slides.length) index = 0;

    galleryCurrentIndex = index;

    slides.forEach(function (slide, i) {
        slide.classList.toggle('active', i === index);
    });

    thumbs.forEach(function (thumb, i) {
        thumb.classList.toggle('active', i === index);
    });

    if (counter) counter.textContent = index + 1;

    // Scroll active thumbnail into view
    var activeThumb = document.querySelector('.gallery-thumb.active');
    if (activeThumb) {
        activeThumb.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'center' });
    }
}

function galleryNext() {
    galleryGoTo(galleryCurrentIndex + 1);
}

function galleryPrev() {
    galleryGoTo(galleryCurrentIndex - 1);
}

// Pause autoplay on hover
document.addEventListener('DOMContentLoaded', function () {
    if (document.querySelector('.gallery-slide')) {
        galleryInit();

        var galleryMain = document.querySelector('.gallery-main');
        if (galleryMain) {
            galleryMain.addEventListener('mouseenter', function () {
                if (galleryAutoPlayInterval) clearInterval(galleryAutoPlayInterval);
            });
            galleryMain.addEventListener('mouseleave', function () {
                if (galleryAutoPlayInterval) clearInterval(galleryAutoPlayInterval);
                galleryAutoPlayInterval = setInterval(galleryNext, 6000);
            });
        }
    }
});

// Keyboard navigation (← →)
document.addEventListener('keydown', function (e) {
    if (!document.querySelector('.gallery-slide')) return;

    if (e.key === 'ArrowLeft') galleryPrev();
    if (e.key === 'ArrowRight') galleryNext();
});