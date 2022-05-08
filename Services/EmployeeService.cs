using API.Core.Models;
using API.Core.Repository;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace API.Services
{
    public class EmployeeService
    {
        private readonly IDesignationRepository _designationRepository;
        private readonly ICountyRepository _countyRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;
        public EmployeeService(ILogger<EmployeeService> logger,
            IDesignationRepository designationRepository,
            ICountyRepository countyRepository,
            IEmployeeRepository employeeRepository)
        {
            _designationRepository = designationRepository;
            _countyRepository = countyRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }
        public async Task<string> ProcessExcel(IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                List<Employee> employees = new List<Employee>();
                List<County> county = new List<County>();
                List<Designation> designations = new List<Designation>();
                #region Process exel
                using (var stream = file.OpenReadStream())
                {
                    HSSFWorkbook xssWorkbook = new HSSFWorkbook(stream);
                    ISheet sheet = xssWorkbook.GetSheetAt(0);
                    IRow headerRow = sheet.GetRow(0);
                    int cellCount = headerRow.LastCellNum;
                    for (int k = 0; k < xssWorkbook.NumberOfSheets; k++)
                    {
                        sheet = xssWorkbook.GetSheetAt(k);
                        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            if (row == null) continue;
                            if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                            switch (k)
                            {
                                case 0:
                                    employees.Add(new Employee
                                    {
                                        Id = Convert.ToInt32(row.GetCell(0).ToString()),
                                        Address1 = row.GetCell(2).ToString(),
                                        Address2 = row.GetCell(3).ToString(),
                                        County = new County { Id = Convert.ToInt32(row.GetCell(4).ToString()) },
                                        PostCode = row.GetCell(5).ToString(),
                                        Designation = new Designation { ID = Convert.ToInt32(row.GetCell(6).ToString()) },
                                        Name = row.GetCell(1).ToString()

                                    });
                                    break;
                                case 1:
                                    county.Add(new County
                                    {
                                        Id = Convert.ToInt32(row.GetCell(0).ToString()),
                                        Name = row.GetCell(1).ToString()
                                    });
                                    break;
                                case 2:
                                    designations.Add(new Designation
                                    {
                                        ID = Convert.ToInt32(row.GetCell(0).ToString()),
                                        Name = row.GetCell(1).ToString()
                                    });
                                    break;
                            }
                        }

                    }
                }
                #endregion
                if (designations != null && designations.Count > 0)
                {
                    foreach (var designation in designations)
                    {
                        await _designationRepository.Add(designation, cancellationToken);
                    }
                }
                else
                    throw new ArgumentNullException(nameof(designations));
                if (county != null && county.Count > 0)
                {
                    foreach (var oneCounty in county)
                    {
                        await _countyRepository.Add(oneCounty, cancellationToken);
                    }
                }
                else
                    throw new ArgumentNullException(nameof(county));
                if (employees != null && employees.Count > 0)
                {
                    foreach (var one in employees)
                    {
                        await _employeeRepository.Add(one, cancellationToken);
                    }
                }

                return "";
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(e.HResult), e, "{Message}", e.Message);
                return e.Message;
            }
        }

        public async Task<object> GetEmployees(int pageRequested, int pageSize, CancellationToken cancellationToken)
        {
            try
            {
                if (pageRequested <= 0)
                    throw new ArgumentException("Number of rows requested is not valid.");
                int start = 0; //offset starting position
                if (pageRequested > 1)
                    start = (pageRequested - 1) * pageSize; //to find the starting position
                int totalCount = await _employeeRepository.GetCount(cancellationToken);
                if (totalCount <= 0)
                    throw new Exception("No items found.");
                int totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
                if (pageRequested > totalCount)
                    throw new Exception("Page not found.");
                IEnumerable<Employee> employees = await _employeeRepository.GetEmployees(start, pageSize, cancellationToken);
                if (employees != null && employees.Any())
                {
                    return new
                    {
                        data = employees.Select(f => new
                        {
                            id = f.Id,
                            name = f.Name,
                            addr1 = f.Address1,
                            addr2 = f.Address2,
                            postCode = f.PostCode,
                            county = f.County.Name,
                            designation = f.Designation.Name
                        }),
                        totalPage
                    };
                }
                else
                    throw new Exception("No result found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(ex.HResult), ex, "{Message}", ex.Message);
                return new { data = "", totalPage = 0 };
            }
        }
    }
}
