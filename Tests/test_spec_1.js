describe('open the calendar', function() {
    it('should display the calendar', function() {
        browser.get('http://localhost:56996/ReservationBookDTO/Details/0');

        element(by.css('.fc-next-button')).click();


    })
});