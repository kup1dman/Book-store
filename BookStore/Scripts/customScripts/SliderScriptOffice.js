$(document).ready(function () {
    let position = 0;
    const slideToShow = 4;
    const slideToScroll = 1;
    const container = $('.slider-container');
    const track = $('.slider-track-officestuff');
    const item = $('.slider-item-officestuff');
    const btnPrev = $('.btn-prev-officestuff');
    const btnNext = $('.btn-next-officestuff');
    const itemsCount = item.length;
    const itemWidth = container.width() / slideToShow;
    const movePosition = slideToScroll * itemWidth;

    item.each(function (index, item) {
        $(item).css({
            minWidth: itemWidth,
        })
    })

    btnNext.click(function () {
        const itemsLeft = itemsCount - (Math.abs(position) + slideToShow * itemWidth) / itemWidth;

        position -= itemsLeft > slideToScroll ? movePosition : itemsLeft * itemWidth;
        setPosition();
        checkButtons();
    })

    btnPrev.click(function () {
        const itemsLeft = Math.abs(position) / itemWidth;

        position += itemsLeft > slideToScroll ? movePosition : itemsLeft * itemWidth;
        setPosition();
        checkButtons();

    })

    const setPosition = () => {
        track.css({
            transform: `translateX(${position}px)`
        });
    };

    const checkButtons = () => {

        btnPrev.prop('disabled', position === 0);
        btnNext.prop(
            'disabled',
            position <= -(itemsCount - slideToShow) * itemWidth
        );
    };

    checkButtons();
})