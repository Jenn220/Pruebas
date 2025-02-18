
using Integration.Orchestrator.Backend.Domain.Commons;
using Integration.Orchestrator.Backend.Domain.Entities.Configurator;
using Integration.Orchestrator.Backend.Domain.Entities.Configurator.Interfaces;
using Integration.Orchestrator.Backend.Domain.Exceptions;
using Integration.Orchestrator.Backend.Domain.Models;
using Integration.Orchestrator.Backend.Domain.Models.Configurador.Catalog;
using Integration.Orchestrator.Backend.Domain.Ports.Configurator;
using Integration.Orchestrator.Backend.Domain.Resources;
using Integration.Orchestrator.Backend.Domain.Services.Configurator;
using Integration.Orchestrator.Backend.Domain.Specifications;
using Moq;
using System.Linq.Expressions;

namespace Integration.Orchestrator.Backend.Domain.Tests.Services.Configurator
{
    public class CatalogServiceTest
    {
        private readonly Mock<ICatalogRepository<CatalogEntity>> _mockRepo;

        private readonly Mock<IAdapterRepository<AdapterEntity>> _mockadapterRepo;
        private readonly Mock<IEntitiesRepository<EntitiesEntity>> _mockentitiesRepo;
        private readonly Mock<IProcessRepository<ProcessEntity>> _mockprocessRepo;
        private readonly Mock<IPropertyRepository<PropertyEntity>> _mockpropertyRepo;
        private readonly Mock<IRepositoryRepository<RepositoryEntity>> _mockrepositoryRepo;
        private readonly Mock<IServerRepository<ServerEntity>> _mockserverRepository;
        private readonly Mock<IStatusService<StatusEntity>> _mockStatusService;


        private readonly CatalogService _service;
        public CatalogServiceTest()
        {
            _mockRepo = new Mock<ICatalogRepository<CatalogEntity>>();

            _mockadapterRepo = new Mock<IAdapterRepository<AdapterEntity>>();
            _mockentitiesRepo = new Mock<IEntitiesRepository<EntitiesEntity>>();
            _mockprocessRepo = new Mock<IProcessRepository<ProcessEntity>>();
            _mockpropertyRepo = new Mock<IPropertyRepository<PropertyEntity>>();
            _mockrepositoryRepo = new Mock<IRepositoryRepository<RepositoryEntity>>();
            _mockserverRepository = new Mock<IServerRepository<ServerEntity>>();
            _mockStatusService = new Mock<IStatusService<StatusEntity>>();
            _service = new CatalogService(_mockRepo.Object, _mockadapterRepo.Object, _mockentitiesRepo.Object, _mockprocessRepo.Object, _mockpropertyRepo.Object, _mockrepositoryRepo.Object, _mockserverRepository.Object, _mockStatusService.Object);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallRepositoryDeleteAsync()
        {
            var entity = new CatalogEntity
            {
                catalog_code = 10,
                catalog_name = "Catlog",
                catalog_value = "value",
                father_code = 1,
                catalog_detail = "details of catalog",
                status_id = Guid.NewGuid()
            };
            await _service.DeleteAsync(entity);
            _mockRepo.Verify(repo => repo.DeleteAsync(entity), Times.Once);
        }

        [Fact]
        public async Task GetAllPaginatedAsync_ShouldReturnPaginatedCatalogEntities_OrderByAscendingAndDescending()
        {
            var paginatedModelAsc = new PaginatedModel()
            {
                First = 0,
                Rows = 10,
                Search = "Catalog",
                Sort_field = "code",  // Ordena por un campo específico
                Sort_order = SortOrdering.Ascending,
                activeOnly = true
            };

            var paginatedModelDesc = new PaginatedModel()
            {
                First = 0,
                Rows = 10,
                Search = "Catalog",
                Sort_field = "code",  // Ordena por el mismo campo
                Sort_order = SortOrdering.Descending,
                activeOnly = true
            };

            var catalog1 = new CatalogResponseModel
            {
                Id = Guid.NewGuid(),
                catalog_code = 10,
                catalog_name = "Catalog1",
                catalog_value = "value1",
                father_code = 1,
                catalog_detail = "details of catalog 1",
                status_id = Guid.NewGuid(),
                is_father = true,
                created_at = DateTime.Now.ToString(),
                updated_at = DateTime.Now.ToString()
            };

            var catalog2 = new CatalogResponseModel
            {
                Id = Guid.NewGuid(),
                catalog_code = 20,
                catalog_name = "Catalog2",
                catalog_value = "value2",
                father_code = 1,
                catalog_detail = "details of catalog 2",
                status_id = Guid.NewGuid(),
                is_father = true,
                created_at = DateTime.Now.ToString(),
                updated_at = DateTime.Now.ToString()
            };

            var catalogs = new List<CatalogResponseModel> { catalog1, catalog2 };

            var specAsc = new CatalogSpecification(paginatedModelAsc);
            var specDesc = new CatalogSpecification(paginatedModelDesc);

            _mockRepo.Setup(repo => repo.GetAllAsync(It.IsAny<ISpecification<CatalogEntity>>()))
                     .ReturnsAsync(catalogs);

            // Verificación del orden ascendente
            var resultAsc = await _service.GetAllPaginatedAsync(paginatedModelAsc);
            var orderedAsc = resultAsc.OrderBy(c => c.catalog_code).ToList();
            Assert.Equal(orderedAsc, resultAsc);
            _mockRepo.Verify(repo => repo.GetAllAsync(It.IsAny<CatalogSpecification>()), Times.Once);

            // Verificación del orden descendente
            var resultDesc = await _service.GetAllPaginatedAsync(paginatedModelDesc);
            _mockRepo.Verify(repo => repo.GetAllAsync(It.IsAny<CatalogSpecification>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetAllPaginatedAsync_ShouldReturnPaginatedCatalogEntities_SortFieldEmpty()
        {
            var paginatedModelAsc = new PaginatedModel()
            {
                First = 0,
                Rows = 10,
                Search = "Catalog",
                Sort_field = "",
                Sort_order = SortOrdering.Ascending,
                activeOnly = true
            };

            var catalog1 = new CatalogResponseModel
            {
                Id = Guid.NewGuid(),
                catalog_code = 10,
                catalog_name = "Catalog1",
                catalog_value = "value1",
                father_code = 1,
                catalog_detail = "details of catalog 1",
                status_id = Guid.NewGuid(),
                is_father = true,
                created_at = DateTime.Now.ToString(),
                updated_at = DateTime.Now.ToString()
            };

            var catalog2 = new CatalogResponseModel
            {
                Id = Guid.NewGuid(),
                catalog_code = 20,
                catalog_name = "Catalog2",
                catalog_value = "value2",
                father_code = 1,
                catalog_detail = "details of catalog 2",
                status_id = Guid.NewGuid(),
                is_father = true,
                created_at = DateTime.Now.ToString(),
                updated_at = DateTime.Now.ToString()
            };

            var catalogs = new List<CatalogResponseModel> { catalog1, catalog2 };

            var specAsc = new CatalogSpecification(paginatedModelAsc);

            _mockRepo.Setup(repo => repo.GetAllAsync(It.IsAny<ISpecification<CatalogEntity>>()))
                     .ReturnsAsync(catalogs);

            // Verificación del orden ascendente
            var resultAsc = await _service.GetAllPaginatedAsync(paginatedModelAsc);
            var orderedAsc = resultAsc.OrderBy(c => c.catalog_code).ToList();
            Assert.Equal(orderedAsc, resultAsc);
            _mockRepo.Verify(repo => repo.GetAllAsync(It.IsAny<CatalogSpecification>()), Times.Once);

        }

        [Fact]
        public async Task GetByFatherAsync_ShouldReturnCatalogEntitiesByFatherCode()
        {
            var father_code = 1;
            var catalog = new CatalogEntity
            {
                catalog_code = 10,
                catalog_name = "Catlog",
                catalog_value = "value",
                father_code = father_code,
                catalog_detail = "details of catalog",
                status_id = Guid.NewGuid()
            };
            var catalogs = new List<CatalogEntity> { catalog };
            var specification = CatalogSpecification.GetByFatherExpression(father_code);
            _mockRepo.Setup(repo => repo.GetByFatherAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                     .ReturnsAsync(catalogs);

            var result = await _service.GetByFatherAsync(father_code);

            Assert.Equal(catalogs, result);
            _mockRepo.Verify(repo => repo.GetByFatherAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()), Times.Once);

        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCatalogEntity_WhenIdExists()
        {
            var id = Guid.NewGuid();
            var catalog = new CatalogEntity
            {
                catalog_code = 10,
                catalog_name = "Catlog",
                catalog_value = "value",
                father_code = 1,
                catalog_detail = "details of catalog",
                status_id = Guid.NewGuid()
            };
            var expression = CatalogSpecification.GetByIdExpression(id);
            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                .ReturnsAsync(catalog);

            var result = await _service.GetByIdAsync(id);

            Assert.Equal(catalog, result);
            _mockRepo.Verify(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetTotalRowsAsync_ShouldReturnTotalNumberOfRows()
        {
            var paginatedModel = new PaginatedModel()
            {
                First = 1,
                Rows = 1,
                Search = "",
                Sort_field = "",
                Sort_order = SortOrdering.Ascending
            };
            var totalRows = 10L;
            var spec = new CatalogSpecification(paginatedModel);
            _mockRepo.Setup(repo => repo.GetTotalRows(It.IsAny<ISpecification<CatalogEntity>>())).ReturnsAsync(totalRows);
            var result = await _service.GetTotalRowsAsync(paginatedModel);
            Assert.Equal(totalRows, result);
            _mockRepo.Verify(repo => repo.GetTotalRows(It.IsAny<CatalogSpecification>()), Times.Once);
        }




        [Fact]
        public async Task InsertAsync_ShouldInsertCatalogEntity_WhenStatusExists()
        {
            var catalog = new CatalogEntity
            {
                catalog_code = 10,
                catalog_name = "Catlog",
                catalog_value = "value",
                father_code = 1,
                catalog_detail = "details of catalog",
                status_id = Guid.NewGuid(),
                is_father = false,
                id = Guid.NewGuid(),
            };

            // Simula que el estado existe y está activo
            _mockStatusService.Setup(repo => repo.GetByIdAsync(catalog.status_id))
                              .ReturnsAsync(new StatusEntity { });

            _mockStatusService.Setup(repo => repo.GetStatusIsActiveAsync(catalog.status_id))
                              .ReturnsAsync(true);

            // Simula que NO existe otro catálogo con el mismo código o nombre en el mismo padre
            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                     .ReturnsAsync((CatalogEntity)null); // Retorna `null` para que pase la validación

            // Simula que el padre sí existe (si lo tiene)
            _mockRepo.Setup(repo => repo.GetByFatherAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                     .ReturnsAsync(new List<CatalogEntity>
                     {
                 new CatalogEntity
                 {
                     id = Guid.NewGuid(),
                     catalog_code = catalog.father_code ?? 0, // Asegurar código válido
                     catalog_name = "ParentCatalog",
                     is_father = true,
                     status_id = Guid.NewGuid() // ID de estado aleatorio
                 }
                     });

            // Corrección importante: Permitir que la validación del padre pase**
            _mockRepo.Setup(repo => repo.GetByFatherAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                     .ReturnsAsync(new List<CatalogEntity>()); // Retorna una lista vacía para que pase `ValidateInactiveCatalogFather`

            // Simula que NO hay relaciones activas con otros módulos
            _mockadapterRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<AdapterEntity, bool>>>()))
                            .ReturnsAsync((AdapterEntity)null);
            _mockentitiesRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<EntitiesEntity, bool>>>()))
                             .ReturnsAsync((EntitiesEntity)null);
            _mockprocessRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<ProcessEntity, bool>>>()))
                            .ReturnsAsync((ProcessEntity)null);
            _mockpropertyRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<PropertyEntity, bool>>>()))
                             .ReturnsAsync((PropertyEntity)null);
            _mockserverRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<ServerEntity, bool>>>()))
                                 .ReturnsAsync((ServerEntity)null);
            _mockrepositoryRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<RepositoryEntity, bool>>>()))
                               .ReturnsAsync((RepositoryEntity)null);

            // Ejecutar la prueba**
            await _service.InsertAsync(catalog);

            // Verificar que el catálogo se insertó correctamente**
            _mockRepo.Verify(repo => repo.InsertAsync(catalog), Times.Once);
        }





        [Fact]
        public async Task UpdateAsync_ShouldUpdateCatalogEntity_WhenStatusExists()
        {
            var catalog = new CatalogEntity
            {
                catalog_code = 10,
                catalog_name = "Catlog",
                catalog_value = "value",
                father_code = 1,
                catalog_detail = "details of catalog",
                status_id = Guid.NewGuid()
            };
            _mockStatusService.Setup(repo => repo.GetByIdAsync(catalog.status_id)).ReturnsAsync(new StatusEntity { });

            await _service.UpdateAsync(catalog);
            _mockRepo.Verify(repo => repo.UpdateAsync(catalog), Times.Once);
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldReturnCatalogEntity_WhenCodeExists()
        {
            // Arrange
            var catalogCode = 10;
            var catalog = new CatalogEntity
            {
                catalog_code = catalogCode,
                catalog_name = "CatalogName",
                catalog_value = "CatalogValue",
                father_code = 1,
                catalog_detail = "Catalog details",
                status_id = Guid.NewGuid()
            };

            // Configura el mock para que retorne la entidad de catálogo cuando se busca por código
            _mockRepo.Setup(repo => repo.GetByCodeAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                     .ReturnsAsync(catalog);

            // Act
            var result = await _service.GetByCodeAsync(catalogCode);

            // Assert
            Assert.Equal(catalog, result);
            _mockRepo.Verify(repo => repo.GetByCodeAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()), Times.Once);
        }



        [Fact]
        public async Task EnsureStatusExists_ShouldThrowOrchestratorArgumentException_WhenStatusDoesNotExist()
        {
            // Arrange
            var nonExistentStatusId = Guid.NewGuid(); // ID de estado inexistente
            var catalogEntity = new CatalogEntity
            {
                catalog_code = 10,
                catalog_name = "CatalogName",
                catalog_value = "CatalogValue",
                father_code = 1,
                catalog_detail = "Catalog details",
                status_id = nonExistentStatusId // ID de estado que no existe
            };

            // Simulamos que el estado NO existe
            _mockStatusService.Setup(repo => repo.GetByIdAsync(nonExistentStatusId))
                              .ReturnsAsync((StatusEntity)null); // Retorna `null` para forzar la excepción

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OrchestratorArgumentException>(() => _service.InsertAsync(catalogEntity));

            // Verificamos que la excepción contiene los valores esperados
            Assert.Equal((int)ResponseCode.NotFoundSuccessfully, exception.Details.Code);
            Assert.Equal(AppMessages.Application_StatusNotFound, exception.Details.Description);
            Assert.Equal(nonExistentStatusId, exception.Details.Data);

            // Verificamos que se intentó buscar el estado en el servicio
            _mockStatusService.Verify(repo => repo.GetByIdAsync(nonExistentStatusId), Times.Once);
        }


        public class CatalogServiceTests
        {
            private readonly Mock<ICatalogRepository<CatalogEntity>> _mockCatalogRepository;
            private readonly Mock<IAdapterRepository<AdapterEntity>> _mockAdapterRepository;
            private readonly Mock<IEntitiesRepository<EntitiesEntity>> _mockEntitiesRepository;
            private readonly Mock<IProcessRepository<ProcessEntity>> _mockProcessRepository;
            private readonly Mock<IPropertyRepository<PropertyEntity>> _mockPropertyRepository;
            private readonly Mock<IRepositoryRepository<RepositoryEntity>> _mockRepositoryRepository;
            private readonly Mock<IServerRepository<ServerEntity>> _mockServerRepository;
            private readonly Mock<IStatusService<StatusEntity>> _mockStatusService;

            private readonly CatalogService _catalogService;

            public CatalogServiceTests()
            {
                _mockCatalogRepository = new Mock<ICatalogRepository<CatalogEntity>>();
                _mockAdapterRepository = new Mock<IAdapterRepository<AdapterEntity>>();
                _mockEntitiesRepository = new Mock<IEntitiesRepository<EntitiesEntity>>();
                _mockProcessRepository = new Mock<IProcessRepository<ProcessEntity>>();
                _mockPropertyRepository = new Mock<IPropertyRepository<PropertyEntity>>();
                _mockRepositoryRepository = new Mock<IRepositoryRepository<RepositoryEntity>>();
                _mockServerRepository = new Mock<IServerRepository<ServerEntity>>();
                _mockStatusService = new Mock<IStatusService<StatusEntity>>();

                _catalogService = new CatalogService(
                    _mockCatalogRepository.Object,
                    _mockAdapterRepository.Object,
                    _mockEntitiesRepository.Object,
                    _mockProcessRepository.Object,
                    _mockPropertyRepository.Object,
                    _mockRepositoryRepository.Object,
                    _mockServerRepository.Object,
                    _mockStatusService.Object
                );
            }


            [Fact]
            public async Task EnsureUniqueCatalogCode_ShouldThrowException_WhenCodeExists()
            {
                // Arrange
                var catalogEntity = new CatalogEntity { catalog_code = 123 };

                _mockCatalogRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                    .ReturnsAsync(new CatalogEntity());

                // Act & Assert
                await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                    _catalogService.InsertAsync(catalogEntity));
            }

            [Fact]
            public async Task EnsureUniqueCatalogName_ShouldThrowException_WhenCatalogNameAlreadyExists()
            {
                // Arrange
                var catalogEntity = new CatalogEntity { catalog_name = "TestName" };

                _mockCatalogRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                    .ReturnsAsync(new CatalogEntity());

                // Act & Assert
                await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                    _catalogService.InsertAsync(catalogEntity));
            }

            [Fact]
            public async Task ValidateParentRules_ShouldThrowException_WhenFatherCodeIsNotNull_ForParentEntity()
            {
                // Arrange
                var catalogEntity = new CatalogEntity { is_father = true, father_code = 123 };

                // Act & Assert
                await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                    _catalogService.InsertAsync(catalogEntity));
            }



            [Fact]
            public async Task ValidateCatalog_ShouldThrowException_WhenCodeExists()
            {
                // Arrange
                var catalogEntity = new CatalogEntity { catalog_code = 123 };
                _mockCatalogRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                    .ReturnsAsync(new CatalogEntity());

                // Act & Assert
                await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                    _catalogService.InsertAsync(catalogEntity));
            }

            [Fact]
            public async Task ValidateCatalog_ShouldThrowException_WhenFatherCodeIsNotNull_ForParentEntity()
            {
                // Arrange
                var catalogEntity = new CatalogEntity { is_father = true, father_code = 123 };

                // Act & Assert
                await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                    _catalogService.InsertAsync(catalogEntity));
            }

            [Fact]
            public async Task ValidateCatalog_ShouldThrowException_WhenCatalogNameAlreadyExists()
            {
                // Arrange
                var catalogEntity = new CatalogEntity { catalog_name = "TestName" };
                _mockCatalogRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                    .ReturnsAsync(new CatalogEntity());

                // Act & Assert
                await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                    _catalogService.InsertAsync(catalogEntity));
            }

            [Fact]
            public async Task ValidateCatalog_ShouldThrowException_WhenFatherCodeIsNull()
            {
                // Arrange
                var catalogEntity = new CatalogEntity { father_code = null };

                // Act & Assert
                await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                    _catalogService.InsertAsync(catalogEntity));
            }

            [Fact]
            public async Task ValidateCatalogStatus_ShouldThrowException_WhenStatusIsInactive()
            {
                // Arrange
                var catalogEntity = new CatalogEntity
                {
                    catalog_code = 10,
                    catalog_name = "CatalogName",
                    catalog_value = "CatalogValue",
                    father_code = 1,
                    catalog_detail = "Catalog details",
                    status_id = Guid.NewGuid()
                };

                // Simulamos que el estado existe pero está inactivo
                var inactiveStatus = new StatusEntity { id = catalogEntity.status_id, status_key = "INACTIVE" };
                _mockStatusService
                    .Setup(repo => repo.GetStatusIsActiveAsync(catalogEntity.status_id))
                    .ReturnsAsync(false);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                    _catalogService.InsertAsync(catalogEntity));

                // Verificamos el mensaje de error esperado
                Assert.Equal((int)ResponseCode.NotFoundSuccessfully, exception.Details.Code);
                Assert.Equal(AppMessages.Domain_ResponseCode_NotActivatedDueToInactiveRelationship, exception.Details.Description);
            }

            [Fact]
            public async Task ValidateParentRules_ShouldThrowException_WhenEntityIsFatherAndHasFatherCode()
            {
                // Arrange
                var catalogEntity = new CatalogEntity
                {
                    is_father = true,
                    father_code = 123 // Esto no debería permitirse
                };

                // Act & Assert
                await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                    _catalogService.InsertAsync(catalogEntity));
            }

            [Fact]
            public async Task ValidateParentRules_ShouldThrowException_WhenEntityIsNotFatherAndFatherCodeIsNull()
            {
                // Arrange
                var catalogEntity = new CatalogEntity
                {
                    is_father = false,
                    father_code = null // Esto no debería permitirse
                };

                // Act & Assert
                await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                    _catalogService.InsertAsync(catalogEntity));
            }

            [Fact]
            public async Task ValidateParentRules_ShouldPass_WhenEntityIsNotFatherAndHasValidFatherCode()
            {
                // Arrange
                var catalogEntity = new CatalogEntity
                {
                    is_father = false,
                    father_code = 123 // Debe ser válido
                };

                // Act
                var exception = await Record.ExceptionAsync(() => _catalogService.InsertAsync(catalogEntity));

                // Assert
                Assert.Null(exception);
            }




            [Fact]
            public async Task ValidateCatalogStatus_ShouldPass_WhenCatalogDoesNotExist()
            {
                // Arrange
                var catalogEntity = new CatalogEntity { id = Guid.NewGuid() };

                _mockCatalogRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                    .ReturnsAsync((CatalogEntity)null); // Simula que el catálogo NO existe

                // Act
                var exception = await Record.ExceptionAsync(() => _catalogService.InsertAsync(catalogEntity));

                // Assert
                Assert.Null(exception);
            }

            [Fact]
            public async Task ValidateCatalogStatus_ShouldPass_WhenCatalogExistsAndIsActive()
            {
                // Arrange
                var catalogEntity = new CatalogEntity { id = Guid.NewGuid(), status_id = Guid.NewGuid() };
                var existingCatalog = new CatalogEntity { id = catalogEntity.id, status_id = catalogEntity.status_id };

                _mockCatalogRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                    .ReturnsAsync(existingCatalog);

                _mockStatusService
                    .Setup(repo => repo.GetStatusIsActiveAsync(catalogEntity.status_id))
                    .ReturnsAsync(true); // Estado activo

                // Act
                var exception = await Record.ExceptionAsync(() => _catalogService.InsertAsync(catalogEntity));

                // Assert
                Assert.Null(exception);
            }

            [Fact]
            public async Task ValidateCatalogStatus_ShouldThrowException_WhenCatalogExistsAndIsInactiveWithRelationships()
            {
                // Arrange
                var catalogEntity = new CatalogEntity { id = Guid.NewGuid(), status_id = Guid.NewGuid() };
                var existingCatalog = new CatalogEntity { id = catalogEntity.id, status_id = Guid.NewGuid() };

                _mockCatalogRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                    .ReturnsAsync(existingCatalog);

                _mockStatusService
                    .Setup(repo => repo.GetStatusIsActiveAsync(catalogEntity.status_id))
                    .ReturnsAsync(false); // Estado inactivo

                _mockAdapterRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<AdapterEntity, bool>>>()))
                    .ReturnsAsync(new AdapterEntity()); // Simula que tiene relaciones activas

                // Act & Assert
                var exception = await Assert.ThrowsAsync<OrchestratorArgumentException>(() =>
                    _catalogService.InsertAsync(catalogEntity));

                Assert.Equal((int)ResponseCode.NotFoundSuccessfully, exception.Details.Code);
                Assert.Equal(AppMessages.Domain_ResponseCode_NotDeleteDueToRelationship, exception.Details.Description);
            }

            [Fact]
            public async Task ValidateCatalogStatus_ShouldPass_WhenCatalogExistsAndIsInactiveWithoutRelationships()
            {
                // Arrange
                var catalogEntity = new CatalogEntity { id = Guid.NewGuid(), status_id = Guid.NewGuid() };
                var existingCatalog = new CatalogEntity { id = catalogEntity.id, status_id = Guid.NewGuid() };

                _mockCatalogRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<CatalogEntity, bool>>>()))
                    .ReturnsAsync(existingCatalog);

                _mockStatusService
                    .Setup(repo => repo.GetStatusIsActiveAsync(catalogEntity.status_id))
                    .ReturnsAsync(false); // Estado inactivo

                _mockAdapterRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Expression<Func<AdapterEntity, bool>>>()))
                    .ReturnsAsync((AdapterEntity)null); // Simula que NO tiene relaciones activas

                // Act
                var exception = await Record.ExceptionAsync(() => _catalogService.InsertAsync(catalogEntity));

                // Assert
                Assert.Null(exception);
            }

        }
    }
}





