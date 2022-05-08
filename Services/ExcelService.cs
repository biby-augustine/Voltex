using API.Core.Models;
using API.Core.Repository;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace API.Services
{
    public class ExcelService
    {
        private readonly IDesignationRepository _designationRepository;
        private readonly ICountyRepository _countyRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<ExcelService> _logger;
        public ExcelService(ILogger<ExcelService> logger,
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
    }
}
