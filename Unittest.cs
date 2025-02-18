using Xunit;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;
using Integration.Orchestrator.Backend.Api.Infrastructure.ServiceRegistrations.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Integration.Orchestrator.Backend.Domain.Commons;
using Integration.Orchestrator.Backend.Domain.Resources;
using Integration.Orchestrator.Backend.Domain.Services.Configurator;
using Integration.Orchestrator.Backend.Domain.Entities.Configurator;
using Integration.Orchestrator.Backend.Domain.Exceptions;
using Integration.Orchestrator.Backend.Domain.Models;
using Integration.Orchestrator.Backend.Domain.Specifications;
using Integration.Orchestrator.Backend.Domain.Entities.Configurator.Interfaces;
using Integration.Orchestrator.Backend.Domain.Entities.ModuleSequence;
using Integration.Orchestrator.Backend.Domain.Ports.Configurator;
using Integration.Orchestrator.Backend.Domain.Models.Configurator;
using Integration.Orchestrator.Backend.Application.Models.Configurator.Integration;
using System.ComponentModel.DataAnnotations;
using Integration.Orchestrator.Backend.Domain.Models.Configurador.Catalog;
using Integration.Orchestrator.Backend.Api.Infrastructure.Extensions;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;



namespace Integration.Orchestrator.Backend.Domain.Tests.Specifications
{
    public class Unittest
    {
        [Fact]
        public void Test_SetupOrdering_DeberiaOrdenarPorCampoEspecifico()
        {
            // Arrange
            var paginatedModel = new PaginatedModel
            {
                Sort_field = "transformation_name",
                Sort_order = SortOrdering.Ascending
            };

            var transformationSpec = new TransformationSpecification(paginatedModel);

            // Act
            var setupOrderingMethod = typeof(TransformationSpecification)
                .GetMethod("SetupOrdering", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            setupOrderingMethod.Invoke(transformationSpec, new object[] { paginatedModel });

            // Assert
            Assert.True(transformationSpec.OrderBy != null || transformationSpec.OrderByDescending != null);

        }

        [Fact]
        public void Test_SetupOrdering_DeberiaOrdenarEnOrdenDescendenteSiSeIndica()
        {
            // Arrange
            var paginatedModel = new PaginatedModel
            {
                Sort_field = "transformation_name",
                Sort_order = SortOrdering.Descending
            };

            var transformationSpec = new TransformationSpecification(paginatedModel);

            // Act
            var setupOrderingMethod = typeof(TransformationSpecification)
                .GetMethod("SetupOrdering", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            setupOrderingMethod.Invoke(transformationSpec, new object[] { paginatedModel });

            // Assert
            Assert.True(transformationSpec.OrderBy != null || transformationSpec.OrderByDescending != null);

        }

        [Fact]
        public void Test_SetupOrdering_DeberiaUsarOrdenPorDefectoSiNoHayCampo()
        {
            // Arrange
            var paginatedModel = new PaginatedModel
            {
                Sort_field = "",
                Sort_order = SortOrdering.Ascending
            };

            var transformationSpec = new TransformationSpecification(paginatedModel);

            // Act
            var setupOrderingMethod = typeof(TransformationSpecification)
                .GetMethod("SetupOrdering", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            setupOrderingMethod.Invoke(transformationSpec, new object[] { paginatedModel });

            // Assert
            Assert.NotNull(transformationSpec.OrderBy);
            var orderByResult = transformationSpec.OrderBy?.Body.ToString();
            Assert.Contains("id", orderByResult);
        }


    }
}








namespace Integration.Orchestrator.Backend.Domain.Tests.Commons
{
    public class ResponseMessageValuesTests
    {
        [Fact]
        public void GetResponseMessage_DeberiaRetornarMensajeEsperado()
        {
            // Arrange
            var responseCode = ResponseCode.CreatedSuccessfully;
            var expectedMessage = AppMessages.Domain_ResponseCode_CreatedSuccessfully;

            // Act
            var actualMessage = ResponseMessageValues.GetResponseMessage(responseCode);

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void GetResponseMessage_DeberiaRetornarMensajeConParametros()
        {
            // Arrange
            var responseCode = ResponseCode.NotFoundSuccessfully;
            var param = "Usuario";
            var expectedMessage = string.Format(AppMessages.Domain_ResponseCode_NotFoundSuccessfully, param);

            // Act
            var actualMessage = ResponseMessageValues.GetResponseMessage(responseCode, param);

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void GetResponseMessage_DeberiaRetornarMensajePorDefectoSiElCodigoNoExiste()
        {
            // Arrange
            var responseCode = (ResponseCode)999; // Código inexistente
            var expectedMessage = AppMessages.Domain_ResponseCode_NotFoundCodeSuccessfully;

            // Act
            var actualMessage = ResponseMessageValues.GetResponseMessage(responseCode);

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void GetResponseMessage_DeberiaRetornarMensajeOriginalSiNoHayParametros()
        {
            // Arrange
            var responseCode = ResponseCode.UpdatedSuccessfully;
            var expectedMessage = AppMessages.Domain_ResponseCode_UpdatedSuccessfully;

            // Act
            var actualMessage = ResponseMessageValues.GetResponseMessage(responseCode);

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void GetResponseMessage_DeberiaRetornarMensajeFormateadoSiHayMultiplesParametros()
        {
            // Arrange
            var responseCode = ResponseCode.NotValidationSuccessfully;
            var expectedMessage = string.Format(AppMessages.Domain_ResponseCode_NotValidationSuccessfully, "Campo1", "Campo2");

            // Act
            var actualMessage = ResponseMessageValues.GetResponseMessage(responseCode, "Campo1", "Campo2");

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void GetResponseMessage_DeberiaRetornarMensajeOriginalSiParametrosEsNull()
        {
            // Arrange
            var responseCode = ResponseCode.CreatedSuccessfully;
            var expectedMessage = AppMessages.Domain_ResponseCode_CreatedSuccessfully;

            // Act
            var actualMessage = ResponseMessageValues.GetResponseMessage(responseCode, null);

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void GetResponseMessage_DeberiaRetornarMensajePorDefectoConParametros()
        {
            // Arrange
            var responseCode = (ResponseCode)999; // Código inexistente
            var expectedMessage = string.Format(AppMessages.Domain_ResponseCode_NotFoundCodeSuccessfully, "ParametroX");

            // Act
            var actualMessage = ResponseMessageValues.GetResponseMessage(responseCode, "ParametroX");

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void GetResponseMessage_DeberiaRetornarMensajesParaTodosLosResponseCodes()
        {
            // Arrange
            var responseCodes = new Dictionary<ResponseCode, string>
            {
                { ResponseCode.CreatedSuccessfully, AppMessages.Domain_ResponseCode_CreatedSuccessfully },
                { ResponseCode.NotCreatedSuccessfully, AppMessages.Domain_ResponseCode_NotCreatedSuccessfully },
                { ResponseCode.UpdatedSuccessfully, AppMessages.Domain_ResponseCode_UpdatedSuccessfully },
                { ResponseCode.NotUpdatedSuccessfully, AppMessages.Domain_ResponseCode_NotUpdatedSuccessfully },
                { ResponseCode.FoundSuccessfully, AppMessages.Domain_ResponseCode_FoundSuccessfully },
                { ResponseCode.NotFoundSuccessfully, AppMessages.Domain_ResponseCode_NotFoundSuccessfully },
                { ResponseCode.DeletedSuccessfully, AppMessages.Domain_ResponseCode_DeletedSuccessfully },
                { ResponseCode.NotDeletedSuccessfully, AppMessages.Domain_ResponseCode_NotDeletedSuccessfully },
                { ResponseCode.NotValidationSuccessfully, AppMessages.Domain_ResponseCode_NotValidationSuccessfully },
                { ResponseCode.NotDeleteDueToRelationship, AppMessages.Domain_ResponseCode_NotDeleteDueToRelationship },
                { ResponseCode.NotActivatedDueToInactiveRelationship, AppMessages.Domain_ResponseCode_NotActivatedDueToInactiveRelationship }
            };

            foreach (var response in responseCodes)
            {
                // Act
                var actualMessage = ResponseMessageValues.GetResponseMessage(response.Key);

                // Assert
                Assert.Equal(response.Value, actualMessage);
            }
        }
    }
}











namespace Integration.Orchestrator.Backend.Domain.Tests
{
    public class GenerateDynamicTextTests
    {
        [Fact]
        public void GenerateDynamicText_ShouldReturnTextWithinRange()
        {
            // Arrange
            int minLength = 10;
            int maxLength = 50;

            // Usar reflexión para acceder a GenerateDynamicText (porque es `static` y `private`)
            MethodInfo method = typeof(IntegrationService).GetMethod("GenerateDynamicText", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            string generatedText = (string)method.Invoke(null, new object[] { minLength, maxLength });

            // Assert
            Assert.NotNull(generatedText);
            Assert.InRange(generatedText.Length, minLength, maxLength);
        }

        [Fact]
        public void GenerateDynamicText_ShouldNotReturnEmptyString_WhenMinLengthGreaterThanZero()
        {
            // Arrange
            int minLength = 5;
            int maxLength = 10;

            // Obtiene el método reflejado de manera segura
            MethodInfo? method = typeof(IntegrationService).GetMethod("GenerateDynamicText", BindingFlags.NonPublic | BindingFlags.Static);

            // Validamos que el método realmente existe, si no existe la prueba debe fallar aquí
            Assert.NotNull(method);

            // Act
            string generatedText = (string)method!.Invoke(null, new object[] { minLength, maxLength });

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(generatedText), "El texto generado es nulo o vacío.");
            Assert.InRange(generatedText.Length, minLength, maxLength);
        }



        [Fact]
        public void GenerateDynamicText_ShouldContainOnlyValidWords()
        {
            // Arrange
            string[] validWords = new[]
            {
                "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit", "sed",
                "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua",
                "ut", "enim", "ad", "minim", "veniam", "quis", "nostrud", "exercitation", "ullamco",
                "laboris", "nisi", "aliquip", "ex", "ea", "commodo", "consequat"
            };

            MethodInfo method = typeof(IntegrationService).GetMethod("GenerateDynamicText", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            string generatedText = (string)method.Invoke(null, new object[] { 10, 50 });

            // Assert
            string[] words = generatedText.Split(' ');
            foreach (var word in words)
            {
                Assert.Contains(word, validWords);
            }
        }
    }
}







public class RegisterApiBehaviorTests
{
    [Fact]
    public void RegisterApiBehavior_Should_ThrowValidationException_On_InvalidModel()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddOptions(); // Asegurar que las opciones están registradas

        var configuration = new ConfigurationBuilder().Build();
        var behavior = new RegisterApiBehavior();
        behavior.RegisterAppServices(services, configuration);

        var provider = services.BuildServiceProvider();
        var optionsMonitor = provider.GetRequiredService<IOptions<ApiBehaviorOptions>>();

        // Validamos que el servicio esté correctamente registrado antes de usarlo
        var options = optionsMonitor.Value;

        var actionContext = new ActionContext();
        actionContext.ModelState.AddModelError("TestField", "ErrorMessage Path");

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
        {
            options.InvalidModelStateResponseFactory(actionContext);
        });

        Assert.Contains("Tipo de dato inválido", exception.Message);
    }

    // Prueba agregada para el constructor Connection
    [Fact]
    public void Connection_Constructor_Should_ThrowException_When_NullPassed()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Connection(null));
    }

    [Fact]
    public void Connection_Constructor_Should_Initialize_Properties_Correctly()
    {
        // Arrange
        var responseModel = new ConnectionResponseModel
        {
            serverName = "localhost",
            adapterName = "SQL",
            repositoryName = "TestDB",
            repositoryPassword = "password123",
            repositoryPort = 5432,
            repositoryUser = "admin"
        };

        // Act
        var connection = new Connection(responseModel);

        // Assert
        Assert.Equal("localhost", connection.server);
        Assert.Equal("SQL", connection.adapter);
        Assert.Equal("TestDB", connection.repository);
        Assert.Equal("password123", connection.password);
        Assert.Equal("5432", connection.port);
        Assert.Equal("admin", connection.user);
    }
}











namespace Integration.Orchestrator.Backend.Domain.Tests.Services.Configurator
{
    public class Unittest
    {
        private readonly Mock<IIntegrationRepository<IntegrationEntity>> _mockIntegrationRepo;
        private readonly Mock<ICatalogRepository<CatalogEntity>> _mockCatalogRepo;
        private readonly Mock<ICodeConfiguratorService> _mockCodeConfiguratorService;
        private readonly Mock<IStatusService<StatusEntity>> _mockStatusService;
        private readonly IntegrationService _integrationService;

        public Unittest()
        {
            _mockIntegrationRepo = new Mock<IIntegrationRepository<IntegrationEntity>>();
            _mockCatalogRepo = new Mock<ICatalogRepository<CatalogEntity>>();
            _mockCodeConfiguratorService = new Mock<ICodeConfiguratorService>();
            _mockStatusService = new Mock<IStatusService<StatusEntity>>();

            _integrationService = new IntegrationService(
                _mockIntegrationRepo.Object,
                _mockCatalogRepo.Object,
                _mockCodeConfiguratorService.Object,
                _mockStatusService.Object
            );
        }

        [Fact]
        public async Task ValidateBussinesLogic_ShouldThrowException_WhenStatusDoesNotExist()
        {
            // Arrange
            var integration = new IntegrationEntity
            {
                status_id = Guid.NewGuid(),
                process = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            // Simula que el estado NO existe
            _mockStatusService.Setup(s => s.GetByIdAsync(integration.status_id))
                .ReturnsAsync((StatusEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                _integrationService.InsertAsync(integration));

            Assert.NotNull(exception);
            Assert.Equal((int)ResponseCode.NotFoundSuccessfully, exception.Details.Code);
            Assert.Equal(AppMessages.Application_StatusNotFound, exception.Details.Description);
            Assert.Equal(integration.status_id, exception.Details.Data);
        }




        [Fact]
        public async Task ValidateBussinesLogic_ShouldThrowException_WhenProcessListHasLessThanTwo()
        {
            // Arrange
            var integration = new IntegrationEntity
            {
                status_id = Guid.NewGuid(),
                process = new List<Guid> { Guid.NewGuid() }
            };

            _mockStatusService.Setup(s => s.GetByIdAsync(integration.status_id))
                .ReturnsAsync(new StatusEntity());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                _integrationService.InsertAsync(integration));

            Assert.NotNull(exception);
            Assert.Equal((int)ResponseCode.NotFoundSuccessfully, exception.Details.Code);
            Assert.Equal(AppMessages.Domain_IntegrationMinTwoRequired, exception.Details.Description);
        }

        [Fact]
        public async Task ValidateBussinesLogic_ShouldThrowException_WhenObservationsExceedMaxLength()
        {
            // Arrange
            var integration = new IntegrationEntity
            {
                status_id = Guid.NewGuid(),
                process = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                integration_observations = new string('A', 256) // Excede el límite
            };

            _mockStatusService.Setup(s => s.GetByIdAsync(integration.status_id))
                .ReturnsAsync(new StatusEntity());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                _integrationService.InsertAsync(integration));

            Assert.NotNull(exception);
            Assert.Equal((int)ResponseCode.NotFoundSuccessfully, exception.Details.Code);
            Assert.Equal(AppMessages.Integration_Observation_Max_length_, exception.Details.Description);
        }


    }
    namespace Integration.Orchestrator.Backend.Domain.Tests.Services.Configurator
    {
        public class CatalogSpecificationTests
        {
            private CatalogSpecification CrearCatalogSpecificationConSortExpressions(PaginatedModel modeloPaginado)
            {
                var especificacionCatalogo = new CatalogSpecification(modeloPaginado);

                // Simulación de sortExpressions para forzar la ejecución de AddFilterSearch
                var sortExpressionsField = typeof(CatalogSpecification)
                    .GetField("sortExpressions", BindingFlags.NonPublic | BindingFlags.Instance);

                var sortExpressions = new Dictionary<string, object>
            {
                { "catalog_name", "mock_filter" },
                { "catalog_value", "mock_filter" }
            };

                sortExpressionsField?.SetValue(especificacionCatalogo, sortExpressions);

                return especificacionCatalogo;
            }

            [Fact]
            public void AgregarBusquedaFiltro_DeberiaAgregarFiltros_CuandoLasOpcionesDeFiltroSonValidas()
            {
                // Arrange
                var modeloPaginado = new PaginatedModel
                {
                    filter_Option = new List<FilterModel>
                {
                    new FilterModel { filter_column = "catalog_name", filter_search = new object[] { "Prueba" } },
                    new FilterModel { filter_column = "catalog_value", filter_search = new object[] { "Valor" } }
                }
                };

                var especificacionCatalogo = CrearCatalogSpecificationConSortExpressions(modeloPaginado);

                // Usamos reflexión para acceder al método privado
                MethodInfo metodoAgregarBusquedaFiltro = typeof(CatalogSpecification)
                    .GetMethod("AddFilterSearch", BindingFlags.NonPublic | BindingFlags.Instance);

                // Act
                metodoAgregarBusquedaFiltro.Invoke(especificacionCatalogo, new object[] { modeloPaginado });

                // Assert
                Assert.True(especificacionCatalogo.AdditionalFilters.ContainsKey("catalog_name"));
                Assert.True(especificacionCatalogo.AdditionalFilters.ContainsKey("catalog_value"));
                Assert.Equal(new object[] { "Prueba" }, especificacionCatalogo.AdditionalFilters["catalog_name"]);
                Assert.Equal(new object[] { "Valor" }, especificacionCatalogo.AdditionalFilters["catalog_value"]);
            }

            [Fact]
            public void AgregarBusquedaFiltro_NoDebeModificarFiltros_CuandoLasOpcionesDeFiltroSonNulasOVacias()
            {
                // Arrange
                var modeloPaginado = new PaginatedModel { filter_Option = null };
                var especificacionCatalogo = CrearCatalogSpecificationConSortExpressions(modeloPaginado);

                MethodInfo metodoAgregarBusquedaFiltro = typeof(CatalogSpecification)
                    .GetMethod("AddFilterSearch", BindingFlags.NonPublic | BindingFlags.Instance);

                // Act
                metodoAgregarBusquedaFiltro.Invoke(especificacionCatalogo, new object[] { modeloPaginado });

                // Assert
                Assert.Empty(especificacionCatalogo.AdditionalFilters);

                // Prueba con lista vacía
                modeloPaginado.filter_Option = new List<FilterModel>();
                metodoAgregarBusquedaFiltro.Invoke(especificacionCatalogo, new object[] { modeloPaginado });

                Assert.Empty(especificacionCatalogo.AdditionalFilters);
            }

            [Fact]
            public void AgregarBusquedaFiltro_NoDebeAgregarFiltros_CuandoColumnasSonInvalidas()
            {
                // Arrange
                var modeloPaginado = new PaginatedModel
                {
                    filter_Option = new List<FilterModel>
                {
                    new FilterModel { filter_column = null, filter_search = new object[] { "Prueba" } },
                    new FilterModel { filter_column = "", filter_search = new object[] { "Valor" } }
                }
                };

                var especificacionCatalogo = CrearCatalogSpecificationConSortExpressions(modeloPaginado);

                MethodInfo metodoAgregarBusquedaFiltro = typeof(CatalogSpecification)
                    .GetMethod("AddFilterSearch", BindingFlags.NonPublic | BindingFlags.Instance);

                // Act
                metodoAgregarBusquedaFiltro.Invoke(especificacionCatalogo, new object[] { modeloPaginado });

                // Assert
                Assert.Empty(especificacionCatalogo.AdditionalFilters);
            }

            [Fact]
            public void AgregarBusquedaFiltro_DeberiaIgnorarFiltros_SiSortExpressionsNoContieneLaClave()
            {
                // Arrange
                var modeloPaginado = new PaginatedModel
                {
                    filter_Option = new List<FilterModel>
                {
                    new FilterModel { filter_column = "non_existent_column", filter_search = new object[] { "No Match" } }
                }
                };

                var especificacionCatalogo = new CatalogSpecification(modeloPaginado); // Aquí no se usa `CrearCatalogSpecificationConSortExpressions`

                MethodInfo metodoAgregarBusquedaFiltro = typeof(CatalogSpecification)
                    .GetMethod("AddFilterSearch", BindingFlags.NonPublic | BindingFlags.Instance);

                // Act
                metodoAgregarBusquedaFiltro.Invoke(especificacionCatalogo, new object[] { modeloPaginado });

                // Assert
                Assert.Empty(especificacionCatalogo.AdditionalFilters);
            }
        }
    }
}
 





