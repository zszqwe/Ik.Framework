(function ($) {
    $.jqgridWarp = $.jqgridWarp || {};

    $.jqgridWarp.init = function (jqGridId, url, height, config, searchConfig) {
        var jqGridPagerId = jqGridId + "_" + "pagers";
        var defaultConfig = {
            url: url,
            datatype: "json",
            mtype: "POST",
            autowidth: true,
            height: height,
            shrinkToFit: false,
            emptyrecords: "",
            viewrecords: true,
            rowNum: 20,
            rowList: [20, 50, 100],
            pagerpos: "right",
            recordpos: "center",
            jsonReader: {
                root: "dataList",
                total: "totalPages",
                page: "pageIndex",
                records: "totalRecords",
                repeatitems: false
            }
        };
        var enablePager = false;
        if (config.pager === false) {
            config.pager = null;
        }
        else {
            $("#" + jqGridId).after('<div id="' + jqGridPagerId + '"></div>');
            enablePager = true;
            config.pager = $('#' + jqGridPagerId);
        }
        config = $.extend(defaultConfig, config || {});
        var $jqgrid = $("#" + jqGridId).jqGrid(config);
        if (enablePager) {
            if (config.searcher === false) {
                $jqgrid.navGrid('#' + jqGridPagerId, { add: false, edit: false, del: false, search: false, refresh: true });
            }
            else {
                searchConfig = $.extend({ multipleSearch: true, groupOps: [{ op: "AND", text: "默认" }, { op: "AND", text: "AND" }] }, searchConfig || {});
                $jqgrid.navGrid('#' + jqGridPagerId, { add: false, edit: false, del: false, search: true, refresh: true }, {}, {}, {}, searchConfig);
            }
        }
    };

    $.jqgridWarp.getRowData = function (jqGridId, id) {
        return $("#" + jqGridId).getRowData(id);
    };

    $.jqgridWarp.getSelectedRowIds = function (jqGridId) {
        return $("#" + jqGridId).getGridParam('selrow');
    }

    $.jqgridWarp.getMultiSelectedRowIds = function (jqGridId) {
        return $("#" + jqGridId).getGridParam('selarrrow');
    }

    $.jqgridWarp.refreshGrid = function (jqGridId) {
        $("#" + jqGridId).trigger("reloadGrid");
    }

    $.jqgridWarp.getGridParam = function (jqGridId, name) {
        return $("#" + jqGridId).jqGrid("getGridParam", name);
    }

    $.jqgridWarp.applyPostData = function (jqGridId, postData)
    {
        var oldPostData = $("#" + jqGridId).jqGrid("getGridParam", "postData");
        var postData = $.extend(true, oldPostData, postData || {});
        $("#" + jqGridId).jqGrid("setGridParam", { postData: postData }).trigger("reloadGrid");
    }

    $.jqgridWarp.setGridParam = function (jqGridId, data)
    {
        $("#" + jqGridId).jqGrid("setGridParam", data);
    }
})(jQuery);

//default formatters
//colModel: [
//    { name: 'id', index: 'id', formatter: customFmatter },
//    { name: 'name', index: 'name', formatter: "showlink", formatoptions: { baseLinkUrl: "save.action", idName: "id", addParam: "&name=123" } },
//    { name: 'price', index: 'price', formatter: "currency", formatoptions: { thousandsSeparator: ",", decimalSeparator: ".", prefix: "$" } },
//    { name: 'email', index: 'email', formatter: "email" },
//    { name: 'amount', index: 'amount', formatter: "number", formatoptions: { thousandsSeparator: ",", defaulValue: "", decimalPlaces: 3 } },
//    { name: 'gender', index: 'gender', formatter: "checkbox", formatoptions: { disabled: false } },
//    { name: 'type', index: 'type', formatter: "select", editoptions: { value: "0:无效;1:正常;2:未知" } }
//]