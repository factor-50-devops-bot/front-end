const siteNavToggle = document.getElementById('site-nav-toggle')

siteNavToggle && siteNavToggle.addEventListener('click', function() {
  $('#site-nav').toggleClass('collapsed');
  $('#site-nav-toggle').toggleClass('collapsed');
});
