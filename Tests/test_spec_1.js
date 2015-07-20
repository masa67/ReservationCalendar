describe('open the calendar', function() {
    it('should display the calendar', function() {
        browser.get('http://localhost:56996/ReservationBookDTO/Details/0');

        element(by.css('.fc-next-button')).click();

        element.all(by.model('model.layerInEdit')).get(2).click();

        element.all(by.css('.fc-slats tr')).get(9).click();
    })
});