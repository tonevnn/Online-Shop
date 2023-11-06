function getAllCate() {
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5024/api/category/getAllCate',
            success: function (result, status, xhr) {
                var select = $("#select-category");
                for (let ele of result) {
                    select.append($("<option>", {
                        value: ele.categoryId,
                        text: ele.categoryName
                    }));
                }
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        }
    );
}

function getProducts() {
    var categoryId = typeof $("#select-category").val() == 'undefined' ? 0 : $("#select-category").val();
    var search = typeof $("#search").val() == 'undefined' ? "" : $("#search").val();
    var accessToken = localStorage.getItem("accessToken");

    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5024/api/product/GetWithFilter?categoryid=' + categoryId + '&search=' + search,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + accessToken);
            },
            success: function (result, status, xhr) {
                var table = $("#orders");
                if ($(".table-row").length) {
                    $("tr").remove(".table-row");
                }
                for (let ele of result) {
                    table.append("<tr class='table-row' >" +
                        "<td><a href='order-detail.html?id='" + ele.productId + ">#" + ele.productId + "</a></td>" +
                        "<td>" + ele.productName + " </td>" +
                        "<td>" + ele.unitPrice + " </td>" +
                        "<td>pieces</td>" +
                        "<td>" + ele.unitsInStock + " </td>" +
                        "<td>" + ele.categoryName + " </td>" +
                        "<td>" + ele.discontinued + " </td>" +
                        "<td>" +
                        "<a href='update-product.html?id=" + ele.productId + "'>Edit</a>  |  " +
                        "<a href='#' data-id=" + ele.productId + " onclick='deleteProduct(this);'>Delete</a>" +
                        "</td>" +
                        "</tr>");
                }
            },
            error: function (xhr, status, error) {
                switch (xhr.status) {
                    case 401:
                        alert("Unauthorized! Please Login!");
                        window.location.href = 'signin.html';
                        break;
                    case 403:
                        alert("Result: user don't have permission !");
                        window.location.href = 'signin.html';
                        break;
                    default:
                        alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
                        window.location.href = 'index.html';
                        break;
                }

            }
        }
    );
}

function getProductbyId(id) {
    var accessToken = localStorage.getItem("accessToken");
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5024/api/product/GetById?id=' + id,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + accessToken);
            },
            success: function (result, status, xhr) {

                $("#productName").val(result.productName);
                $("#unitPrice").val(result.unitPrice);
                $("#quantityPerUnit").val(result.quantityPerUnit);
                $("#unitsInStock").val(result.unitsInStock);
                getProductCategory(result.categoryId);
                $("#reorderLevel").val(result.reorderLevel);
                $("#unitsOnOrder").val(result.unitsOnOrder);
                $("#discontinued").prop('checked', result.discontinued);
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        }
    );
}

function getProductCategory(id) {
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5024/api/category/getAllCate',
            success: function (result, status, xhr) {
                var select = $("#select-category");
                for (let ele of result) {
                    select.append($("<option>", {
                        value: ele.categoryId,
                        text: ele.categoryName
                    }));
                }
                select.val(id);
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        }
    );
}

function updateProduct() {
    var accessToken = localStorage.getItem("accessToken");
    var productName = $("#productName").val();
    var unitPrice = $("#unitPrice").val();
    var quantityPerUnit = $("#quantityPerUnit").val();
    var unitsInStock = $("#unitsInStock").val();
    if (productName == "" || unitPrice == "" || quantityPerUnit == "" || unitsInStock == "") {
        $("#error-product").attr("hidden", productName != "");
        $("#error-unitPrice").attr("hidden", unitPrice != "");
        $("#error-quantity").attr("hidden", quantityPerUnit != "");
        $("#error-unitStock").attr("hidden", unitsInStock != "");
    } else {
        const product = {
            "productId": productId,
            "productName": $("#productName").val(),
            "categoryId": $("#select-category").val(),
            "quantityPerUnit": $("#quantityPerUnit").val(),
            "unitPrice": $("#unitPrice").val(),
            "unitsInStock": $("#unitsInStock").val(),
            "unitsOnOrder": 0,
            "reorderLevel": $("#reorderLevel").val(),
            "discontinued": $("#discontinued").prop('checked')
        };
        let body = JSON.stringify(product);
        $.ajax({
            type: "PUT",
            url: 'http://localhost:5024/api/product/Update',
            data: body,
            contentType: "application/json",
            dataType: 'json',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + accessToken);
            },
            success: function (result) {
                alert("Update Successfully!");
                location.href = 'product.html';
            },
            error: function (xhr, status, error, data) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText + " " + error.text);
            }
        });
    }
}

function addProduct() {
    var accessToken = localStorage.getItem("accessToken");
    var productName = $("#productName").val();
    var unitPrice = $("#unitPrice").val();
    var quantityPerUnit = $("#quantityPerUnit").val();
    var unitsInStock = $("#unitsInStock").val();
    if (productName == "" || unitPrice == "" || quantityPerUnit == "" || unitsInStock == "") {
        $("#error-product").attr("hidden", productName != "");
        $("#error-unitPrice").attr("hidden", unitPrice != "");
        $("#error-quantity").attr("hidden", quantityPerUnit != "");
        $("#error-unitStock").attr("hidden", unitsInStock != "");
    } else {
        var accessToken = localStorage.getItem("accessToken");
        const product = {
            "productName": $("#productName").val(),
            "categoryId": $("#select-category").val(),
            "quantityPerUnit": $("#quantityPerUnit").val(),
            "unitPrice": $("#unitPrice").val(),
            "unitsInStock": $("#unitsInStock").val(),
            "unitsOnOrder": 0,
            "reorderLevel": $("#reorderLevel").val(),
            "discontinued": $("#discontinued").prop('checked')
        };
        let body = JSON.stringify(product);
        $.ajax({
            type: "POST",
            url: 'http://localhost:5024/api/product/Create',
            data: body,
            contentType: "application/json",
            dataType: 'json',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + accessToken);
            },
            success: function (result) {
                alert("Add Success!");
                location.href = 'product.html';
            },
            error: function (xhr, status, error, data) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText + " " + error.text);
            }
        });
    }
}

function deleteProduct(ctl) {
    var accessToken = localStorage.getItem("accessToken");

    var id = $(ctl).data("id");
    $.ajax(
        {
            type: "DELETE",
            url: 'http://localhost:5024/api/product/Delete?id=' + id,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + accessToken);
            },
            success: function (result, status, xhr) {
                alert("Delete success!");
                getAllCate();
                getProducts();
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        }
    );
}


function Logout() {
    localStorage.removeItem("accessToken");
    window.location.href = 'index.html';
}