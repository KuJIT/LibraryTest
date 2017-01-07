Ext.onReady(function () {
    var filteringPanel = Ext.create('Ext.panel', {
        title: 'Фильтрация',
        cls: 'filteringPanel',
        items: [{
            html: 'фильтрация'
        }]
    });
    var userPanel = Ext.create('Ext.panel', {
        title: 'Я',
        cls: 'userPanel',
        items: [{
            html: 'Я'
        }]
    });
    var booksPanel = Ext.create('Ext.panel', {
        title: 'Все книги',
        cls: 'bookPanel',
        items: [{
            html: 'Все книги'
        }]
    });
    var userBookPanel = Ext.create('Ext.panel', {
        title: 'Мои книги',
        cls: 'userBookPanel',
        items: [{
            html: 'Мои книги'
        }]
    });

    var viewport = Ext.create('Ext.container.Viewport', {
        layout: 'border',
        items: [{
            xtype: 'panel',
            border: false,
            header: false,
            region: 'east',
            items: [userBookPanel]
        }]
    });
});