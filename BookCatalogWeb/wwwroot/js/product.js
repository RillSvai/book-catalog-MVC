let dataTable;

$(document).ready(() => {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblProducts').DataTable(
        {
            "ajax":
            {
                url: '/admin/product/getall'
            },
            "columns": [
                { data: 'title', "width": "15%" },
                { data: 'isbn', "width": "15%" },
                { data: 'author', "width": "15%" },
                { data: 'listPrice', "width": "15%" },
                { data: 'category.name', "width": "15%" },
                {
                    data: 'id',
                    "render": (data) =>
                    {
                        return `
                        <div class="btn-group w-100" role="group">
                            <a href="/admin/product/upsert?id=${data}" class="btn btn-primary rounded-start-1">
								<i class="bi bi-pencil-square"></i> Edit
							</a>
							<a onclick=Delete("/admin/product/delete/${data}") class="btn btn-danger rounded-end-1">
								<i class="bi bi-trash3-fill"></i> Delete
							</a>
                        </div>
                        `;
                    },
                    "width": "25%"
                }
                ]
        });
}

function Delete(url)
{
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax(
                {
                    url,
                    type: 'DELETE',
                    success: (data) => {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                });
        }
    })
}