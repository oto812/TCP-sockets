console.log('Web server is running!');

document.addEventListener('DOMContentLoaded', function() {
    console.log('Page loaded successfully');
    
    // Add some interactivity
    const heading = document.querySelector('h1');
    if (heading) {
        heading.addEventListener('click', function() {
            alert('Hello from the TCP Web Server!');
        });
    }
});