let dataTable;

$(document).ready(() => {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('tblCompanies').DataTable(
        {
            "ajax":
            {
                url: '/admin/company/getall'
            },
            "columns": [
                { data: 'name', "width": "15%" },
                { data: 'city', "width": "15%" },
                { data: 'state', "width": "15%" },
                { data: 'postalCode', "width": "15%" },
                { data: 'phoneNumber', "width": "15%" },
                {
                    data: 'id',
                    "render": (data) =>
                    {
                        return `
                        <div class="btn-group w-100" role="group">
                            <a href="/admin/company/upsert?id=${data}" class="btn btn-primary rounded-start-1">
								<i class="bi bi-pencil-square"></i> Edit
							</a>
							<a onclick=Delete("/admin/company/delete/${data}") class="btn btn-danger rounded-end-1">
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