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
  InputGroupText
} from "reactstrap";
import { useNavigate } from "react-router-dom";
import SimpleHeader from "components/Headers/SimpleHeader.js";
import api from "services/api";
import { format } from "date-fns";
import { debounce } from "lodash";

const Customers = () => {
  const navigate = useNavigate();
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [pagination, setPagination] = useState({
    currentPage: 1,
    totalPages: 1,
    totalItems: 0,
  });

  const loadCustomers = async (page = 1) => {
    try {
      setLoading(true);
      setError(null);
      const response = await api.get('/api/customers', {
        params: {
          searchTerm,
          page,
          pageSize: 10,
        },
      });
      
      if (!response.data || !response.data.items) {
        console.error('Invalid response format:', response.data);
        setError('Formato de resposta inválido');
        setCustomers([]);
        return;
      }

      setCustomers(response.data.items);
      setPagination({
        currentPage: response.data.page || 1,
        totalPages: response.data.totalPages || 1,
        totalItems: response.data.totalCount || 0,
      });
    } catch (err) {
      setError('Erro ao carregar clientes.');
      console.error('Error loading customers:', err);
      setCustomers([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCustomers(1);
  }, []);

  useEffect(() => {
    const debouncedLoad = debounce(() => loadCustomers(1), 500);
    debouncedLoad();
    return () => debouncedLoad.cancel();
  }, [searchTerm]);

  const handlePageChange = (page) => {
    loadCustomers(page);
  };

  const formatCpf = (cpf) => {
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, "$1.$2.$3-$4");
  };

  const renderPagination = () => {
    const pages = [];
    for (let i = 1; i <= pagination.totalPages; i++) {
      pages.push(
        <Button
          key={i}
          color={i === pagination.currentPage ? "primary" : "secondary"}
          onClick={() => handlePageChange(i)}
          size="sm"
          className="mx-1"
        >
          {i}
        </Button>
      );
    }
    return pages;
  };

  return (
    <>
      <SimpleHeader name="Clientes" parentName="Gestão" />
      <Container className="mt--6" fluid>
        <Row>
          <Col>
            <Card className="shadow">
              <CardHeader className="border-0">
                <Row className="align-items-center">
                  <Col xs="8">
                    <h3 className="mb-0">Clientes</h3>
                  </Col>
                  <Col className="text-right" xs="4">
                    <Button
                      color="primary"
                      onClick={() => navigate("/admin/customers/new")}
                      size="sm"
                    >
                      Novo Cliente
                    </Button>
                  </Col>
                </Row>
              </CardHeader>
              <CardBody>
                {error && <Alert color="danger">{error}</Alert>}

                <Row className="mb-4">
                  <Col md="4">
                    <InputGroup>
                      <InputGroupAddon addonType="prepend">
                        <InputGroupText>
                          <i className="fas fa-search" />
                        </InputGroupText>
                      </InputGroupAddon>
                      <Input
                        placeholder="Buscar clientes..."
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
                      <th scope="col">CPF</th>
                      <th scope="col">Cidade</th>
                      <th scope="col">Data de Cadastro</th>
                      <th scope="col">Ações</th>
                    </tr>
                  </thead>
                  <tbody>
                    {loading ? (
                      <tr>
                        <td colSpan="5" className="text-center">
                          Carregando...
                        </td>
                      </tr>
                    ) : Array.isArray(customers) && customers.length > 0 ? (
                      customers.map((customer) => (
                        <tr key={customer.id}>
                          <td>{customer.name}</td>
                          <td>{customer.cpf || '-'}</td>
                          <td>{customer.cityName || '-'}</td>
                          <td>
                            {format(
                              new Date(customer.createdAt),
                              'dd/MM/yyyy HH:mm'
                            )}
                          </td>
                          <td>
                            <Button
                              color="info"
                              size="sm"
                              className="mr-2"
                              onClick={() => navigate(`/admin/customers/${customer.id}`)}
                            >
                              <i className="fas fa-eye" />
                            </Button>
                            <Button
                              color="primary"
                              size="sm"
                              onClick={() => navigate(`/admin/customers/${customer.id}/edit`)}
                            >
                              <i className="fas fa-edit" />
                            </Button>
                          </td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td colSpan="5" className="text-center">
                          Nenhum cliente encontrado.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </Table>

                {pagination.totalPages > 1 && (
                  <div className="d-flex justify-content-center mt-4">
                    {renderPagination()}
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

export default Customers; 