$(function () {
    /*Confirm Page delition*/

    $("a.btn-danger").click(function () {
        if (!confirm("Confirm page deletion")) return false;
    });
    //----------------------------
    // sorting Script

    $("table#pages tbody").sortable(
        {
            items: "tr:not(.home)",
            placeholder: "ui-state-highlight",
            update: function () {
                var ids = $("table#pages tbody").sortable("serialize");
                var url = "/Admin/Pages/ReorderPages";
                $.post(url, ids, function (data) {

                });
            }
        });
});