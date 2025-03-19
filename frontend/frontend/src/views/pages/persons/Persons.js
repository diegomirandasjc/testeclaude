import React, { useState, useEffect } from "react";
import {
  Card,
  CardHeader,
  CardBody,
  Container,
  Row,
  Col,
  Table,
  Button,
  Input,
  Alert,
  InputGroup,
  InputGroupAddon,
  InputGroupText,
  Pagination,
  PaginationItem,
  PaginationLink
} from "reactstrap";
import { useNavigate } from "react-router-dom";
import SimpleHeader from "components/Headers/SimpleHeader.js";
import api from "services/api";

const Persons = () => {
  const navigate = useNavigate();
  const [persons, setPersons] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [pagination, setPagination] = useState({
    currentPage: 1,
    totalPages: 1,
    pageSize: 10
  });

  const loadPersons = async (search = "", page = 1) => {
    try {
      setLoading(true);
      setError(null);
      const response = await api.get("/api/persons", {
        params: {
          searchTerm: search,
          page,
          pageSize: pagination.pageSize
        }
      });

      if (!response.data || !response.data.items) {
        console.error('Invalid response format:', response.data);
        setError('Formato de resposta inválido');
        setPersons([]);
        return;
      }

      setPersons(response.data.items);
      setPagination(prev => ({
        ...prev,
        currentPage: response.data.page || 1,
        totalPages: response.data.totalPages || 1,
        totalItems: response.data.totalCount || 0
      }));
    } catch (err) {
      console.error("Erro ao carregar pessoas:", err);
      setError(err.message || "Erro ao carregar pessoas");
      setPersons([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const delayDebounceFn = setTimeout(() => {
      loadPersons(searchTerm, 1);
    }, 500);

    return () => clearTimeout(delayDebounceFn);
  }, [searchTerm]);

  const handlePageChange = (page) => {
    loadPersons(searchTerm, page);
  };

  const renderPagination = () => {
    const pages = [];
    const maxVisiblePages = 5;
    let startPage = Math.max(1, pagination.currentPage - Math.floor(maxVisiblePages / 2));
    let endPage = Math.min(pagination.totalPages, startPage + maxVisiblePages - 1);

    if (endPage - startPage + 1 < maxVisiblePages) {
      startPage = Math.max(1, endPage - maxVisiblePages + 1);
    }

    // Botão "Anterior"
    pages.push(
      <PaginationItem key="prev" disabled={pagination.currentPage === 1}>
        <PaginationLink
          previous
          onClick={() => handlePageChange(pagination.currentPage - 1)}
        />
      </PaginationItem>
    );

    // Primeira página
    if (startPage > 1) {
      pages.push(
        <PaginationItem key={1}>
          <PaginationLink onClick={() => handlePageChange(1)}>1</PaginationLink>
        </PaginationItem>
      );
      if (startPage > 2) {
        pages.push(
          <PaginationItem key="ellipsis1" disabled>
            <PaginationLink>...</PaginationLink>
          </PaginationItem>
        );
      }
    }

    // Páginas visíveis
    for (let i = startPage; i <= endPage; i++) {
      pages.push(
        <PaginationItem key={i} active={i === pagination.currentPage}>
          <PaginationLink onClick={() => handlePageChange(i)}>{i}</PaginationLink>
        </PaginationItem>
      );
    }

    // Última página
    if (endPage < pagination.totalPages) {
      if (endPage < pagination.totalPages - 1) {
        pages.push(
          <PaginationItem key="ellipsis2" disabled>
            <PaginationLink>...</PaginationLink>
          </PaginationItem>
        );
      }
      pages.push(
        <PaginationItem key={pagination.totalPages}>
          <PaginationLink onClick={() => handlePageChange(pagination.totalPages)}>
            {pagination.totalPages}
          </PaginationLink>
        </PaginationItem>
      );
    }

    // Botão "Próximo"
    pages.push(
      <PaginationItem key="next" disabled={pagination.currentPage === pagination.totalPages}>
        <PaginationLink
          next
          onClick={() => handlePageChange(pagination.currentPage + 1)}
        />
      </PaginationItem>
    );

    return pages;
  };

  return (
    <>
      <SimpleHeader name="Pessoas" parentName="Gestão" />
      <Container className="mt--6" fluid>
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <Row className="align-items-center">
                  <Col xs="8">
                    <h3 className="mb-0">Lista de Pessoas</h3>
                  </Col>
                  <Col className="text-right" xs="4">
                    <Button
                      color="primary"
                      onClick={() => navigate("/admin/persons/new")}
                    >
                      Nova Pessoa
                    </Button>
                  </Col>
                </Row>
              </CardHeader>
              <CardBody>
                {error && (
                  <Alert color="danger" toggle={() => setError(null)}>
                    {error}
                  </Alert>
                )}

                <Row className="mb-4">
                  <Col md="4">
                    <InputGroup>
                      <InputGroupAddon addonType="prepend">
                        <InputGroupText>
                          <i className="fas fa-search" />
                        </InputGroupText>
                      </InputGroupAddon>
                      <Input
                        placeholder="Buscar pessoas..."
                        type="text"
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                      />
                    </InputGroup>
                  </Col>
                </Row>

                <Table className="align-items-center table-flush" responsive>
                  <thead className="thead-light">
                    <tr>
                      <th scope="col">Nome</th>
                      <th scope="col">Email</th>
                      <th scope="col">Data de Cadastro</th>
                      <th scope="col">Ações</th>
                    </tr>
                  </thead>
                  <tbody>
                    {loading ? (
                      <tr>
                        <td colSpan="4" className="text-center">
                          Carregando...
                        </td>
                      </tr>
                    ) : persons.length > 0 ? (
                      persons.map((person) => (
                        <tr key={person.id}>
                          <td>{person.name}</td>
                          <td>{person.email}</td>
                          <td>
                            {new Date(person.createdAt).toLocaleDateString('pt-BR', {
                              day: '2-digit',
                              month: '2-digit',
                              year: 'numeric',
                              hour: '2-digit',
                              minute: '2-digit'
                            })}
                          </td>
                          <td>
                            <Button
                              color="info"
                              size="sm"
                              className="mr-2"
                              onClick={() => navigate(`/admin/persons/${person.id}`)}
                            >
                              <i className="fas fa-eye" />
                            </Button>
                            <Button
                              color="primary"
                              size="sm"
                              onClick={() => navigate(`/admin/persons/${person.id}/edit`)}
                            >
                              <i className="fas fa-edit" />
                            </Button>
                          </td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td colSpan="4" className="text-center">
                          Nenhuma pessoa encontrada.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </Table>

                {pagination.totalPages > 1 && (
                  <div className="d-flex justify-content-center mt-4">
                    <Pagination>
                      {renderPagination()}
                    </Pagination>
                  </div>
                )}
              </CardBody>
            </Card>
          </Col>
        </Row>
      </Container>
    </>
  );
};

export default Persons;
